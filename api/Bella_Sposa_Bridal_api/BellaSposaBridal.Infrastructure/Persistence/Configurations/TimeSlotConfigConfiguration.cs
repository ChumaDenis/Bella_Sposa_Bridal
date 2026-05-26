using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class TimeSlotConfigConfiguration : IEntityTypeConfiguration<TimeSlotConfig>
{
    public void Configure(EntityTypeBuilder<TimeSlotConfig> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();
        builder.Property(t => t.Time).IsRequired().HasMaxLength(5);
    }
}
