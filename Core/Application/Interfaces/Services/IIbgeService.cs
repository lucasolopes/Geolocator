using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IIbgeService
{
    Task<List<IbgeRegionDto>> GetRegionsFromIbgeAsync();
    Task<List<State>> GetStatesAsync();
    Task<List<Mesoregion>> GetMesoregionsAsync(int stateId);
    Task<List<MicroRegion>> GetMicroregionsAsync(int mesoregionId);
    Task<List<Municipality>> GetMunicipalitiesAsync(int microregionId);
    Task<List<Districts>> GetDistrictsAsync(int municipalityId);
    Task<List<SubDistricts>> GetSubDistrictsAsync(int districtId);
}
