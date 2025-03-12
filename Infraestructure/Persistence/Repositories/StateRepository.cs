using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class StateRepository(GeolocatorDbContext context) : IStateRepository
{
    private readonly GeolocatorDbContext _context = context;

    public IUnitOfWork UnitOfWork => _context;
    public async Task<HashSet<long>> GetAllIdsAsync()
    {
        return new HashSet<long>(await _context.States.Select(s => s.Id).ToListAsync());
    }

    public async Task AddRangeAsync(List<State> entities)
    {
        await _context.States.AddRangeAsync(entities);
    }

    public async Task<State> AddAsync(State entity)
    {
        await _context.States.AddAsync(entity);
        return entity;
    }

    public Task DeleteAsync(State entity)
    {
        _context.States.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<State>> GetAllAsync()
    {
        return await _context.States.ToListAsync();
    }

    public async Task<State> GetByIdAsync(long id)
    {
        return await _context.States.FindAsync(id);
    }

    public async Task<List<State>> GetByIdsWithRelationshipsAsync(List<long> ids)
    {
        return await _context.States
            .Where(state => ids.Contains(state.Id))
            .Include(state => state.Region)
            .Include(state => state.Mesoregions)
            .ToListAsync();
    }

    public Task UpdateAsync(State entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
