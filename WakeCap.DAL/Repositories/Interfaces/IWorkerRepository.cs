using WakeCap.DAL.Entities;

namespace WakeCap.DAL.Repositories.Interfaces;

public interface IWorkerRepository
{
    Task<Dictionary<string, int>> GetWorkerCodesAndIdsAsync(HashSet<string> workerCodes);
}
