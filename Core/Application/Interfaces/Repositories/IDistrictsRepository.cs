using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IDistrictsRepository : IRepository<Districts>
{
    Task<List<Districts>> GetAllAsync();
    Task<Districts> GetByIdAsync(long id);
    Task<List<Districts>> GetByMunicipalityIdAsync(long municipalityId);
    Task<List<Districts>> GetByIdsWithRelationshipsAsync(List<long> ids);
}
