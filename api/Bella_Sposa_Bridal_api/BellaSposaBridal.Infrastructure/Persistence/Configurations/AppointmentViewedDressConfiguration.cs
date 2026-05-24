using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class AppointmentViewedDressConfiguration : IEntityTypeConfiguration<AppointmentViewedDress>
{
    public void Configure(EntityTypeBuilder<AppointmentViewedDress> builder)
    {
        builder.HasKey(vd => new { vd.AppointmentId, vd.Order });

        builder.HasOne(vd => vd.Dress)
            .WithMany()
            .HasForeignKey(vd => vd.DressId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
