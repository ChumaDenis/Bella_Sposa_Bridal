using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class SilhouetteTypeConfiguration : IEntityTypeConfiguration<SilhouetteType>
{
    public void Configure(EntityTypeBuilder<SilhouetteType> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.DisplayOrder).HasDefaultValue(0);
    }
}
