using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class MunicipalityRepository(GeolocatorDbContext context) : IMunicipalityRepository
{
    private readonly GeolocatorDbContext _context = context;

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Municipality> AddAsync(Municipality entity)
    {
        await _context.Municipalities.AddAsync(entity);
        return entity;
    }

    public Task DeleteAsync(Municipality entity)
    {
        _context.Municipalities.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<Municipality>> GetAllAsync()
    {
        return await _context.Municipalities.ToListAsync();
    }

    public async Task<Municipality> GetByIdAsync(int id)
    {
        return await _context.Municipalities.FindAsync(id);
    }

    public async Task<List<Municipality>> GetByMicroRegionIdAsync(int microRegionId)
    {
        return await _context.Municipalities
            .Where(m => m.MicroRegionId == microRegionId)
            .ToListAsync();
    }

    public Task UpdateAsync(Municipality entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
