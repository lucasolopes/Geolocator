using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IStateRepository : IRepository<State>
{
    Task<List<State>> GetAllAsync();
    Task<State> GetByIdAsync(int id);
}
