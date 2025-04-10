﻿using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class MunicipalityRepository(GeolocatorDbContext context) : IMunicipalityRepository
{
    private readonly GeolocatorDbContext _context = context;

    public IUnitOfWork UnitOfWork => _context;

    public async Task<HashSet<long>> GetAllIdsAsync()
    {
        return new HashSet<long>(await _context.Municipalities.Select(m => m.Id).ToListAsync());
    }

    public async Task AddRangeAsync(List<Municipality> entities)
    {
        await _context.Municipalities.AddRangeAsync(entities);
    }

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

    public async Task<Municipality> GetByIdAsync(long id)
    {
        return await _context.Municipalities.FindAsync(id);
    }

    public async Task<List<Municipality>> GetByMicroRegionIdAsync(long microRegionId)
    {
        return await _context.Municipalities
            .Where(m => m.MicroRegionId == microRegionId)
            .ToListAsync();
    }

    public async Task<List<Municipality>> GetByIdsWithRelationshipsAsync(List<long> ids)
    {
        return await _context.Municipalities
            .Where(municipality => ids.Contains(municipality.Id))
            .Include(municipality => municipality.MicroRegion)
            .Include(municipality => municipality.Districts)
            .ToListAsync();
    }

    public Task UpdateAsync(Municipality entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
