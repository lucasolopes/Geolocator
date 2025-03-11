using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class DistrictsRepository(GeolocatorDbContext context) : IDistrictsRepository
{
    private readonly GeolocatorDbContext _context = context;

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Districts> AddAsync(Districts entity)
    {
        await _context.Districts.AddAsync(entity);
        return entity;
    }

    public Task DeleteAsync(Districts entity)
    {
        _context.Districts.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<Districts>> GetAllAsync()
    {
        return await _context.Districts.ToListAsync();
    }

    public async Task<Districts> GetByIdAsync(long id)
    {
        return await _context.Districts.FindAsync(id);
    }

    public async Task<List<Districts>> GetByMunicipalityIdAsync(long municipalityId)
    {
        return await _context.Districts
            .Where(d => d.MunicipalityId == municipalityId)
            .ToListAsync();
    }

    public async Task<HashSet<long>> GetAllIdsAsync()
    {
        return new HashSet<long>(await _context.Districts.AsNoTracking().Select(e => e.Id).ToListAsync());
    }

    public async Task AddRangeAsync(List<Districts> newDistricts)
    {
        await _context.Districts.AddRangeAsync(newDistricts);
    }

    public Task UpdateAsync(Districts entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
