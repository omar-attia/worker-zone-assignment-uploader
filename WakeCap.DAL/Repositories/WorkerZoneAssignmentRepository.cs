using Microsoft.EntityFrameworkCore;
using System.Transactions;
using WakeCap.DAL.Entities;
using WakeCap.DAL.Repositories.Interfaces;

namespace WakeCap.DAL.Repositories;

public class WorkerZoneAssignmentRepository(WakeCapDbContext wakeCapDbContext): IWorkerZoneAssignmentRepository
{
    public async Task BulkInsertAssignmentsAsync(IEnumerable<WorkerZoneAssignment> assignments)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await wakeCapDbContext.BulkInsertAsync(assignments.ToList(), options =>
        {
            options.BatchSize = 1000;
            options.SetOutputIdentity = false;
        });

        scope.Complete();
    }

    public async Task<bool> ExistsAssignmentAsync(int workerId, int zoneId, DateTime assignmentDate)
    {
        return await wakeCapDbContext.WorkerZoneAssignments
            .AnyAsync(a => a.WorkerId == workerId
                        && a.ZoneId == zoneId
                        && a.EffectiveDate == assignmentDate);
    }

    public async Task<Dictionary<(string, DateTime), string>> GetExistingAssignmentsAsync(HashSet<(string WorkerCode, DateTime AssignmetDate)> rawAssignmentKeys)
    {
        if (!rawAssignmentKeys.Any())
            return [];

        // Load only potentially matching rows (based on WorkerCode)
        var candidateAssignments = await (
            from assignment in wakeCapDbContext.WorkerZoneAssignments.AsNoTracking()
            join worker in wakeCapDbContext.Workers on assignment.WorkerId equals worker.Id
            join zone in wakeCapDbContext.Zones on assignment.ZoneId equals zone.Id
            where rawAssignmentKeys.Select(k => k.WorkerCode).Contains(worker.Code)
            select new
            {
                WorkerCode = worker.Code,
                EffectiveDate = assignment.EffectiveDate.Date,
                ZoneCode = zone.Code
            }
        ).ToListAsync();

        // filtering to match both WorkerCode and EffectiveDate
        var result = candidateAssignments
            .Where(x => rawAssignmentKeys.Contains((x.WorkerCode, x.EffectiveDate)))
            .ToDictionary(
                x => (x.WorkerCode, x.EffectiveDate.Date),
                x => x.ZoneCode
            );

        return result;
    }




}
