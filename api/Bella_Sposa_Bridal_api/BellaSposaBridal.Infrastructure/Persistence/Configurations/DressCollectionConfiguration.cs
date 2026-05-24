using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class DressCollectionConfiguration : IEntityTypeConfiguration<DressCollection>
{
    public void Configure(EntityTypeBuilder<DressCollection> builder)
    {
        builder.HasKey(dc => new { dc.DressId, dc.CollectionId });

        builder.HasOne(dc => dc.Dress)
            .WithMany(d => d.Collections)
            .HasForeignKey(dc => dc.DressId);

        builder.HasOne(dc => dc.Collection)
            .WithMany(c => c.DressCollections)
            .HasForeignKey(dc => dc.CollectionId);
    }
}
