using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WakeCap.DAL.Entities;

namespace WakeCap.DAL.Configurations;

public class UploadLogConfiguration : IEntityTypeConfiguration<UploadLog>
{
    public void Configure(EntityTypeBuilder<UploadLog> builder)
    {
        builder.ToTable("upload_log");

        builder.HasKey(ul => ul.Id);

        builder.Property(ul => ul.FileName)
              .HasColumnName("file_name")
              .IsRequired();

        builder.Property(ul => ul.TotalRows)
               .HasColumnName("total_rows")
               .IsRequired();

        builder.Property(ul => ul.ErrorRows)
               .HasColumnName("error_rows")
               .IsRequired();

        builder.Property(ul => ul.ValidRows)
              .HasColumnName("valid_rows")
              .IsRequired();

        builder.Property(ul => ul.Status)
              .HasColumnName("status")
              .IsRequired();

        builder.HasIndex(ul => ul.UploadedAt);
    }
}
