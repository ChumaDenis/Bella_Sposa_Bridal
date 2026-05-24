using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class DressConfiguration : IEntityTypeConfiguration<Dress>
{
    public void Configure(EntityTypeBuilder<Dress> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Tagline)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.Description)
            .IsRequired();

        builder.Property(d => d.Material)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.CorsetType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Color)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Silhouette)
            .IsRequired();
    }
}
