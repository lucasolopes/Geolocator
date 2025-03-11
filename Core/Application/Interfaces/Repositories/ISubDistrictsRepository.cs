using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISubDistrictsRepository : IRepository<SubDistricts>
{
    Task<List<SubDistricts>> GetAllAsync();
    Task<SubDistricts> GetByIdAsync(long id);
    Task<List<SubDistricts>> GetByDistrictIdAsync(long districtId);
}
