using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IDistrictsRepository : IRepository<Districts>
{
    Task<List<Districts>> GetAllAsync();
    Task<Districts> GetByIdAsync(int id);
    Task<List<Districts>> GetByMunicipalityIdAsync(int municipalityId);
}
