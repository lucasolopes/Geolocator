using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISubDistrictsRepository : IRepository<SubDistricts>
{
    Task<List<SubDistricts>> GetAllAsync();
    Task<SubDistricts> GetByIdAsync(int id);
    Task<List<SubDistricts>> GetByDistrictIdAsync(int districtId);
}
