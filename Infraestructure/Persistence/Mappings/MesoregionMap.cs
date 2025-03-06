using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings;

public class MesoregionMap : IEntityTypeConfiguration<Mesoregion>
{
    public void Configure(EntityTypeBuilder<Mesoregion> builder)
    {
        builder.ToTable("Mesoregions");

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasColumnType("varchar(150)")
            .IsRequired();
    }
}
