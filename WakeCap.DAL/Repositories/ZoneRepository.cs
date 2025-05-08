using Microsoft.EntityFrameworkCore;
using WakeCap.DAL.Interfaces;
using WakeCap.DAL.Repositories.Interfaces;

namespace WakeCap.DAL.Repositories;

public class ZoneRepository(IWakeCapDbContext wakeCapDbContext): IZoneRepository
{
    public async Task<Dictionary<string, int>> GetZoneCodesAndIdsAsync(HashSet<string> zoneCodes)
    {
        return await wakeCapDbContext.Zones
            .Where(z=> zoneCodes.Contains(z.Code))
            .Select(z => new { z.Code, z.Id })
            .ToDictionaryAsync(z => z.Code, z => z.Id);
    }
}
