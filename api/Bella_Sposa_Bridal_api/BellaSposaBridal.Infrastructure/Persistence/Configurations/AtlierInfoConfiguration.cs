using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellaSposaBridal.Infrastructure.Persistence.Configurations;

public class AtlierInfoConfiguration : IEntityTypeConfiguration<AtlierInfo>
{
    public void Configure(EntityTypeBuilder<AtlierInfo> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Address)
            .IsRequired();

        builder.Property(a => a.Phone)
            .IsRequired()
            .HasMaxLength(50);
    }
}
