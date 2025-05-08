using WakeCap.DAL.Entities;

namespace WakeCap.DAL.Repositories.Interfaces
{
    public interface IZoneRepository
    {
        Task<Dictionary<string, int>> GetZoneCodesAndIdsAsync(HashSet<string> zoneCodes);
    }
}
