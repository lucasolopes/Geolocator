using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class SubDistrictsRepository(GeolocatorDbContext context) : ISubDistrictsRepository
{
    private readonly GeolocatorDbContext _context = context;

    public IUnitOfWork UnitOfWork => _context;
    public async Task<HashSet<long>> GetAllIdsAsync()
    {
        return new HashSet<long>(await _context.Set<SubDistricts>().Select(s => s.Id).ToListAsync());
    }

    public async Task AddRangeAsync(List<SubDistricts> entities)
    {
        await _context.Set<SubDistricts>().AddRangeAsync(entities);
    }

    public async Task<SubDistricts> AddAsync(SubDistricts entity)
    {
        await _context.Set<SubDistricts>().AddAsync(entity);
        return entity;
    }

    public Task DeleteAsync(SubDistricts entity)
    {
        _context.Set<SubDistricts>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<SubDistricts>> GetAllAsync()
    {
        return await _context.Set<SubDistricts>().ToListAsync();
    }

    public async Task<SubDistricts> GetByIdAsync(long id)
    {
        return await _context.Set<SubDistricts>().FindAsync(id);
    }

    public async Task<List<SubDistricts>> GetByDistrictIdAsync(long districtId)
    {
        return await _context.Set<SubDistricts>()
            .Where(s => s.DistrictId == districtId)
            .ToListAsync();
    }

    public async Task<List<SubDistricts>> GetByIdsWithRelationshipsAsync(List<long> ids)
    {
        return await _context.Set<SubDistricts>()
            .Where(subDistrict => ids.Contains(subDistrict.Id))
            .Include(subDistrict => subDistrict.District) 
            .ToListAsync();
    }

    public Task UpdateAsync(SubDistricts entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
