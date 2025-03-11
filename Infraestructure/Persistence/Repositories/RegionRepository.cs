using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class RegionRepository(GeolocatorDbContext context) : IRegionRepository
{
    private readonly GeolocatorDbContext _context = context;

    public IUnitOfWork UnitOfWork => _context;
    public async Task<HashSet<long>> GetAllIdsAsync()
    {
        return new HashSet<long>(await _context.Regions.Select(r => r.Id).ToListAsync());
    }

    public async Task AddRangeAsync(List<Region> entities)
    {
        await _context.Regions.AddRangeAsync(entities);
    }

    public async Task<Region> AddAsync(Region entity)
    {
        await _context.Regions.AddAsync(entity);
        return entity;
    }

    public Task DeleteAsync(Region entity)
    {
        _context.Regions.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<Region>> GetAllAsync()
    {
        return await _context.Regions.ToListAsync();
    }

    public async Task<Region> GetByIdAsync(long id)
    {
        return await _context.Regions.FindAsync(id);
    }

    public Task UpdateAsync(Region entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
