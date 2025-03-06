using Domain.Entities;

namespace Persistence;
﻿using Microsoft.EntityFrameworkCore;

public class GeolocatorDbContext(DbContextOptions<GeolocatorDbContext> options) : DbContext(options)
{
    public DbSet<Districts> Districts { get; set; }
    public DbSet<Mesoregion> Mesoregions { get; set; }
    public DbSet<MicroRegion> MicroRegions { get; set; }
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<State> States { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeolocatorDbContext).Assembly);
    }
}
