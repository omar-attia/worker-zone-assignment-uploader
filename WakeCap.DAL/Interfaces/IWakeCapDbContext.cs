using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using WakeCap.DAL.Entities;

namespace WakeCap.DAL.Interfaces;

public interface IWakeCapDbContext
{
    DbSet<Worker> Workers { get; set; }
    DbSet<Zone> Zones { get; set; }
    DbSet<WorkerZoneAssignment> WorkerZoneAssignments { get; set; }
    DbSet<UploadLog> UploadLogs { get; set; }
    Task BulkInsertAsync<T>(IEnumerable<T> entities, Action<BulkConfig>? configure = null) where T : class;
    Task BulkInsertOrUpdateAsync<T>(IEnumerable<T> entities, Action<BulkConfig>? configure = null) where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
