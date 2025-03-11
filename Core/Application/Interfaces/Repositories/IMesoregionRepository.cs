using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IMesoregionRepository : IRepository<Mesoregion>
{
    Task<List<Mesoregion>> GetAllAsync();
    Task<Mesoregion> GetByIdAsync(long id);
    Task<List<Mesoregion>> GetByStateIdAsync(long stateId);
}
