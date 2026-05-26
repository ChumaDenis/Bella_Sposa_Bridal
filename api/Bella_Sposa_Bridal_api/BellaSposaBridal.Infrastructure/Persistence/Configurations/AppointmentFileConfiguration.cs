using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class AppointmentFileConfiguration : IEntityTypeConfiguration<AppointmentFile>
{
    public void Configure(EntityTypeBuilder<AppointmentFile> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileName).IsRequired().HasMaxLength(500);
        builder.Property(f => f.Url).IsRequired();
        builder.Property(f => f.ContentType).IsRequired().HasMaxLength(200);

        builder.HasOne(f => f.Appointment)
            .WithMany(a => a.Files)
            .HasForeignKey(f => f.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
