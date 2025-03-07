using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings;

public class SubDistrictMap : IEntityTypeConfiguration<SubDistricts>
{
    public void Configure(EntityTypeBuilder<SubDistricts> builder)
    {
        builder.ToTable("SubDistricts");

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasMaxLength(150)
            .IsRequired();
            
        builder.Property(p => p.DistrictId)
            .HasColumnType("integer")
            .IsRequired();
    }
}
