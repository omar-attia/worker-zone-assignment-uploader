using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WakeCap.DAL.Entities;

namespace WakeCap.DAL.Configurations;

public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> builder)
    {
        builder.ToTable("worker");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
               .ValueGeneratedOnAdd();

        builder.Property(w => w.Code)
               .HasColumnType("varchar(10)")
               .HasMaxLength(10)
               .IsRequired();

        builder.HasIndex(w => w.Code)
               .IsUnique();

        builder.Property(w => w.Name)
               .HasColumnType("varchar(100)")
               .HasMaxLength(100)
               .IsRequired();

        builder.HasMany(w => w.Assignments)
               .WithOne(a => a.Worker)
               .HasForeignKey(a => a.WorkerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
