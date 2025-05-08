using Microsoft.EntityFrameworkCore;
using WakeCap.DAL.Interfaces;
using WakeCap.DAL.Repositories.Interfaces;

namespace WakeCap.DAL.Repositories;

public class WorkerRepository(IWakeCapDbContext wakeCapDbContext): IWorkerRepository
{
    public async Task<Dictionary<string, int>> GetWorkerCodesAndIdsAsync(HashSet<string> workerCodes)
    {
        return await wakeCapDbContext.Workers
            .Where(z=> workerCodes.Contains(z.Code))
            .Select(z => new { z.Code, z.Id })
            .ToDictionaryAsync(z => z.Code, z => z.Id);
    }

}
