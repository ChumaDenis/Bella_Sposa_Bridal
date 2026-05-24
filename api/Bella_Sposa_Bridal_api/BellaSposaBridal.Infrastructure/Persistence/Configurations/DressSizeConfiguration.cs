using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class DressSizeConfiguration : IEntityTypeConfiguration<DressSize>
{
    public void Configure(EntityTypeBuilder<DressSize> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Size)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(s => s.Dress)
            .WithMany(d => d.Sizes)
            .HasForeignKey(s => s.DressId);
    }
}
