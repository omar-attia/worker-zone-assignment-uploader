using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using WakeCap.DAL.Entities;
using WakeCap.DAL.Interfaces;
using WakeCap.DAL.Configurations;

namespace WakeCap.DAL;

public class WakeCapDbContext : DbContext, IWakeCapDbContext
{
    public WakeCapDbContext(DbContextOptions<WakeCapDbContext> options)
        : base(options)
    {
    }

    public DbSet<Worker> Workers { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<WorkerZoneAssignment> WorkerZoneAssignments { get; set; }
    public DbSet<UploadLog> UploadLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WorkerConfiguration());
        modelBuilder.ApplyConfiguration(new ZoneConfiguration());
        modelBuilder.ApplyConfiguration(new WorkerZoneAssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new UploadLogConfiguration());
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);

    public async Task BulkInsertAsync<T>(IEnumerable<T> entities, Action<BulkConfig>? configure = null) where T : class
    {
        var bulkConfig = new BulkConfig();
        configure?.Invoke(bulkConfig);
        await this.BulkInsertAsync(entities.ToList(), bulkConfig);
    }

    public async Task BulkInsertOrUpdateAsync<T>(IEnumerable<T> entities, Action<BulkConfig>? configure = null) where T : class
    {
        var bulkConfig = new BulkConfig();
        configure?.Invoke(bulkConfig);
        await this.BulkInsertOrUpdateAsync(entities.ToList(), bulkConfig);
    }
}
