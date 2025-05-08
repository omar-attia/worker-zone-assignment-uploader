using WakeCap.DAL.Entities;

namespace WakeCap.DAL.Repositories.Interfaces;

public interface IWorkerZoneAssignmentRepository
{
    Task BulkInsertAssignmentsAsync(IEnumerable<WorkerZoneAssignment> assignments);
    Task<bool> ExistsAssignmentAsync(int workerId, int zoneId, DateTime assignmentDate);
    Task<Dictionary<(string, DateTime), string>> GetExistingAssignmentsAsync(HashSet<(string WorkerCode, DateTime AssignmetDate)> rawAssignmentKeys);
}
