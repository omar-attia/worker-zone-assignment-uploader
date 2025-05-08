using WakeCap.DAL.Entities;
using WakeCap.DAL.Interfaces;
using WakeCap.DAL.Repositories.Interfaces;

namespace WakeCap.DAL.Repositories;

public class UploadLogRepository(IWakeCapDbContext wakeCapDbContext) : IUploadLogRepository
{
    public async Task SaveLogAsync(UploadLog log)
    {
        await wakeCapDbContext.UploadLogs.AddAsync(log);
        await wakeCapDbContext.SaveChangesAsync();
    }
}
