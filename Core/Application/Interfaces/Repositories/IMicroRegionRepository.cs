using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IMicroRegionRepository : IRepository<MicroRegion>
{
    Task<List<MicroRegion>> GetAllAsync();
    Task<MicroRegion> GetByIdAsync(long id);
    Task<List<MicroRegion>> GetByMesoregionIdAsync(long mesoregionId);
}
