using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class MesoregionRepository(GeolocatorDbContext context) : IMesoregionRepository
{
    private readonly GeolocatorDbContext _context = context;

    public IUnitOfWork UnitOfWork => _context;
    public async Task<HashSet<long>> GetAllIdsAsync()
    {
        return new HashSet<long>(await _context.Mesoregions.Select(m => m.Id).ToListAsync());
    }

    public async Task AddRangeAsync(List<Mesoregion> entities)
    {
        await _context.Mesoregions.AddRangeAsync(entities);
    }

    public async Task<Mesoregion> AddAsync(Mesoregion entity)
    {
        await _context.Mesoregions.AddAsync(entity);
        return entity;
    }

    public Task DeleteAsync(Mesoregion entity)
    {
        _context.Mesoregions.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<Mesoregion>> GetAllAsync()
    {
        return await _context.Mesoregions.ToListAsync();
    }

    public async Task<Mesoregion> GetByIdAsync(long id)
    {
        return await _context.Mesoregions.FindAsync(id);
    }

    public async Task<List<Mesoregion>> GetByStateIdAsync(long stateId)
    {
        return await _context.Mesoregions
            .Where(m => m.StateId == stateId)
            .ToListAsync();
    }

    public Task UpdateAsync(Mesoregion entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
