using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings;

public class DistrictsMap : IEntityTypeConfiguration<Districts>
{
    public void Configure(EntityTypeBuilder<Districts> builder)
    {
        builder.ToTable("Districts");

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasColumnType("varchar(150)")
            .IsRequired();
    }
}
