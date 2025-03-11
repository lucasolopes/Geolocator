using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IMunicipalityRepository : IRepository<Municipality>
{
    Task<List<Municipality>> GetAllAsync();
    Task<Municipality> GetByIdAsync(long id);
    Task<List<Municipality>> GetByMicroRegionIdAsync(long microRegionId);
}
