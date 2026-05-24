using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class RelatedDressConfiguration : IEntityTypeConfiguration<RelatedDress>
{
    public void Configure(EntityTypeBuilder<RelatedDress> builder)
    {
        builder.HasKey(r => new { r.DressId, r.RelatedDressId });

        builder.HasOne(r => r.Dress)
            .WithMany(d => d.RelatedDresses)
            .HasForeignKey(r => r.DressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Related)
            .WithMany()
            .HasForeignKey(r => r.RelatedDressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
