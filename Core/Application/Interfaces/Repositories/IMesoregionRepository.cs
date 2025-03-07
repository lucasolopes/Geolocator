using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IMesoregionRepository : IRepository<Mesoregion>
{
    Task<List<Mesoregion>> GetAllAsync();
    Task<Mesoregion> GetByIdAsync(int id);
    Task<List<Mesoregion>> GetByStateIdAsync(int stateId);
}
