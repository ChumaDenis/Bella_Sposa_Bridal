using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class DressPhotoConfiguration : IEntityTypeConfiguration<DressPhoto>
{
    public void Configure(EntityTypeBuilder<DressPhoto> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Url)
            .IsRequired();

        builder.Property(p => p.Type)
            .IsRequired();

        builder.HasOne(p => p.Dress)
            .WithMany(d => d.Photos)
            .HasForeignKey(p => p.DressId);
    }
}
