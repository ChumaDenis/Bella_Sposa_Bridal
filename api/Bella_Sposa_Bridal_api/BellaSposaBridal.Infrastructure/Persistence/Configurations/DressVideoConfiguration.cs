using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class DressVideoConfiguration : IEntityTypeConfiguration<DressVideo>
{
    public void Configure(EntityTypeBuilder<DressVideo> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Url)
            .IsRequired();

        builder.Property(v => v.Type)
            .IsRequired();

        builder.HasOne(v => v.Dress)
            .WithMany(d => d.Videos)
            .HasForeignKey(v => v.DressId);
    }
}
