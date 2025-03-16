using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class GeolocatorDbContext(DbContextOptions<GeolocatorDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Districts> Districts { get; set; }
    public DbSet<Mesoregion> Mesoregions { get; set; }
    public DbSet<MicroRegion> MicroRegions { get; set; }
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<SubDistricts> SubDistricts { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeolocatorDbContext).Assembly);
    }
}
