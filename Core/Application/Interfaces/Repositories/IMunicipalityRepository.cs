using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IMunicipalityRepository : IRepository<Municipality>
{
    Task<List<Municipality>> GetAllAsync();
    Task<Municipality> GetByIdAsync(int id);
    Task<List<Municipality>> GetByMicroRegionIdAsync(int microRegionId);
}
