using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IRegionRepository : IRepository<Region>
{
    Task<List<Region>> GetAllAsync();
    Task<Region> GetByIdAsync(long id);
    Task<List<Region>> GetByIdsWithRelationshipsAsync(List<long> ids);
}
