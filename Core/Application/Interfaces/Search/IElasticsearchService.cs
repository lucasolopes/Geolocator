using Domain.Entities;

namespace Application.Interfaces.Search;

public interface IElasticsearchService
{
    Task CreateIndicesIfNotExistAsync();
    Task<bool> IndexRegionsAsync(IEnumerable<Region> regions);
    Task<bool> IndexStatesAsync(IEnumerable<State> states);
    Task<bool> IndexMesoregionsAsync(IEnumerable<Mesoregion> mesoregions);
    Task<bool> IndexMicroRegionsAsync(IEnumerable<MicroRegion> microRegions);
    Task<bool> IndexMunicipalitiesAsync(IEnumerable<Municipality> municipalities);
    Task<bool> IndexDistrictsAsync(IEnumerable<Districts> districts);
    Task<bool> IndexSubDistrictsAsync(IEnumerable<SubDistricts> subDistricts);

    Task<IEnumerable<Region>> SearchRegionsByNameAsync(string searchTerm, int page = 1, int pageSize = 10);
    Task<IEnumerable<State>> SearchStatesByNameAsync(string searchTerm, int page = 1, int pageSize = 10);
    Task<IEnumerable<Mesoregion>> SearchMesoregionsByNameAsync(string searchTerm, int page = 1, int pageSize = 10);
    Task<IEnumerable<MicroRegion>> SearchMicroRegionsByNameAsync(string searchTerm, int page = 1, int pageSize = 10);
    Task<IEnumerable<Municipality>> SearchMunicipalitiesByNameAsync(string searchTerm, int page = 1, int pageSize = 10);
    Task<IEnumerable<Districts>> SearchDistrictsByNameAsync(string searchTerm, int page = 1, int pageSize = 10);
    Task<IEnumerable<SubDistricts>> SearchSubDistrictsByNameAsync(string searchTerm, int page = 1, int pageSize = 10);
}
