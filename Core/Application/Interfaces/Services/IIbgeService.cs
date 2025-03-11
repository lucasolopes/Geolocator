using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IIbgeService
{
    Task<List<Region>> GetRegionsAsync();
    Task<List<State>> GetStatesAsync();
    Task<List<Mesoregion>> GetMesoregionsAsync();
    Task<List<MicroRegion>> GetMicroregionsAsync();
    Task<List<Municipality>> GetMunicipalitiesAsync();
    Task<List<Districts>> GetDistrictsAsync();
    Task<List<SubDistricts>> GetSubDistrictsAsync();
}
