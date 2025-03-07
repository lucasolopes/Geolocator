using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class StateRepository(GeolocatorDbContext context) : IStateRepository
{
    private readonly GeolocatorDbContext _context = context;

    public IUnitOfWork UnitOfWork => _context;

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

    public async Task<State> GetByIdAsync(int id)
    {
        return await _context.States.FindAsync(id);
    }

    public Task UpdateAsync(State entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
