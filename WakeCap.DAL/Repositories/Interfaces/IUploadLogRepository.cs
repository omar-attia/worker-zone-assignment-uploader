using WakeCap.DAL.Entities;

namespace WakeCap.DAL.Repositories.Interfaces;

public interface IUploadLogRepository
{
    Task SaveLogAsync(UploadLog log);
}
