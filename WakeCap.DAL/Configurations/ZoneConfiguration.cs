using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WakeCap.DAL.Entities;

namespace WakeCap.DAL.Configurations;

public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.ToTable("zone");

        builder.HasKey(z => z.Id);

        builder.Property(z => z.Id)
               .ValueGeneratedOnAdd();

        builder.Property(z => z.Code)
               .HasColumnType("varchar(10)")
               .HasMaxLength(10)
               .IsRequired();

        builder.HasIndex(z => z.Code)
               .IsUnique();

        builder.Property(z => z.Name)
               .HasColumnType("varchar(100)")
               .HasMaxLength(100)
               .IsRequired();

        builder.HasMany(z => z.Assignments)
               .WithOne(a => a.Zone)
               .HasForeignKey(a => a.ZoneId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
