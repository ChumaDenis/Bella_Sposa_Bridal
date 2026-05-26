using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class AppointmentTypeConfigConfiguration : IEntityTypeConfiguration<AppointmentTypeConfig>
{
    public void Configure(EntityTypeBuilder<AppointmentTypeConfig> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Price).HasColumnType("decimal(10,2)");
        builder.Property(t => t.Description).HasMaxLength(500);
    }
}
