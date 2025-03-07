using Domain.Entities;
using MediatR;

namespace Application.Queries.Search;

public class SearchLocationsQuery : IRequest<SearchLocationsResult>
{
    public string SearchTerm { get; }
    public int Page { get; }
    public int PageSize { get; }
    public bool IncludeRegions { get; }
    public bool IncludeStates { get; }
    public bool IncludeMesoregions { get; }
    public bool IncludeMicroRegions { get; }
    public bool IncludeMunicipalities { get; }
    public bool IncludeDistricts { get; }
    public bool IncludeSubDistricts { get; }

    public SearchLocationsQuery(
        string searchTerm,
        int page = 1,
        int pageSize = 10,
        bool includeRegions = true,
        bool includeStates = true,
        bool includeMesoregions = true,
        bool includeMicroRegions = true,
        bool includeMunicipalities = true,
        bool includeDistricts = true,
        bool includeSubDistricts = true)
    {
        SearchTerm = searchTerm;
        Page = page;
        PageSize = pageSize;
        IncludeRegions = includeRegions;
        IncludeStates = includeStates;
        IncludeMesoregions = includeMesoregions;
        IncludeMicroRegions = includeMicroRegions;
        IncludeMunicipalities = includeMunicipalities;
        IncludeDistricts = includeDistricts;
        IncludeSubDistricts = includeSubDistricts;
    }
}

public class SearchLocationsResult 
{
    public IEnumerable<Region> Regions { get; }
    public IEnumerable<State> States { get; }
    public IEnumerable<Mesoregion> Mesoregions { get; }
    public IEnumerable<MicroRegion> MicroRegions { get; }
    public IEnumerable<Municipality> Municipalities { get; }
    public IEnumerable<Districts> Districts { get; }
    public IEnumerable<SubDistricts> SubDistricts { get; }

    public SearchLocationsResult(
        IEnumerable<Region> regions,
        IEnumerable<State> states,
        IEnumerable<Mesoregion> mesoregions,
        IEnumerable<MicroRegion> microRegions,
        IEnumerable<Municipality> municipalities,
        IEnumerable<Districts> districts,
        IEnumerable<SubDistricts> subDistricts)
    {
        Regions = regions;
        States = states;
        Mesoregions = mesoregions;
        MicroRegions = microRegions;
        Municipalities = municipalities;
        Districts = districts;
        SubDistricts = subDistricts;
    }
}
