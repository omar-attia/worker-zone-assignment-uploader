using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using WakeCap.DAL.Entities;

namespace WakeCap.DAL.Configurations;

public class WorkerZoneAssignmentConfiguration : IEntityTypeConfiguration<WorkerZoneAssignment>
{
    public void Configure(EntityTypeBuilder<WorkerZoneAssignment> builder)
    {
        builder.ToTable("worker_zone_assignment");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
               .ValueGeneratedOnAdd();

        builder.Property(a => a.EffectiveDate)
               .HasColumnName("effective_date")
        .IsRequired();

        builder.Property(e => e.EffectiveDate)
       .HasColumnType("date");

        builder.Property(a => a.WorkerId)
               .HasColumnName("worker_id")
               .IsRequired();

        builder.Property(a => a.ZoneId)
               .HasColumnName("zone_id")
               .IsRequired();

        builder.HasIndex(a => new { a.WorkerId, a.ZoneId, a.EffectiveDate })
               .IsUnique();

        builder.HasOne(a => a.Worker)
               .WithMany(w => w.Assignments)
               .HasForeignKey(a => a.WorkerId);

        builder.HasOne(a => a.Zone)
               .WithMany(z => z.Assignments)
               .HasForeignKey(a => a.ZoneId);
    }
}
