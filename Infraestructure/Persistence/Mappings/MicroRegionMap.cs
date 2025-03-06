using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings;

public class MicroRegionMap : IEntityTypeConfiguration<MicroRegion>
{
    public void Configure(EntityTypeBuilder<MicroRegion> builder)
    {
        builder.ToTable("MicroRegions");

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasColumnType("varchar(150)")
            .IsRequired();
    }
}
