using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class MicroRegionRepository(GeolocatorDbContext context) : IMicroRegionRepository
{
    private readonly GeolocatorDbContext _context = context;

    public IUnitOfWork UnitOfWork => _context;

    public async Task<MicroRegion> AddAsync(MicroRegion entity)
    {
        await _context.MicroRegions.AddAsync(entity);
        return entity;
    }

    public Task DeleteAsync(MicroRegion entity)
    {
        _context.MicroRegions.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<MicroRegion>> GetAllAsync()
    {
        return await _context.MicroRegions.ToListAsync();
    }

    public async Task<MicroRegion> GetByIdAsync(int id)
    {
        return await _context.MicroRegions.FindAsync(id);
    }

    public async Task<List<MicroRegion>> GetByMesoregionIdAsync(int mesoregionId)
    {
        return await _context.MicroRegions
            .Where(m => m.MesoregionId == mesoregionId)
            .ToListAsync();
    }

    public Task UpdateAsync(MicroRegion entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
