using Application.Interfaces.Search;
using Application.Queries.Search;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Queries.Handlers;

public class SearchLocationsQueryHandler : IRequestHandler<SearchLocationsQuery, SearchLocationsResult>
{
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<SearchLocationsQueryHandler> _logger;

    public SearchLocationsQueryHandler(
        IElasticsearchService elasticsearchService,
        ILogger<SearchLocationsQueryHandler> logger)
    {
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    public async Task<SearchLocationsResult> Handle(SearchLocationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando localizações com o termo '{SearchTerm}'", request.SearchTerm);

        try
        {
            // Resultados padrão vazios
            IEnumerable<Region> regions = Enumerable.Empty<Region>();
            IEnumerable<State> states = Enumerable.Empty<State>();
            IEnumerable<Mesoregion> mesoregions = Enumerable.Empty<Mesoregion>();
            IEnumerable<MicroRegion> microRegions = Enumerable.Empty<MicroRegion>();
            IEnumerable<Municipality> municipalities = Enumerable.Empty<Municipality>();
            IEnumerable<Districts> districts = Enumerable.Empty<Districts>();
            IEnumerable<SubDistricts> subDistricts = Enumerable.Empty<SubDistricts>();

            // Executa as buscas selecionadas em paralelo
            var tasks = new List<Task>();

            if (request.IncludeRegions)
            {
                tasks.Add(Task.Run(async () =>
                {
                    regions = await _elasticsearchService.SearchRegionsByNameAsync(
                        request.SearchTerm, request.Page, request.PageSize);
                }, cancellationToken));
            }

            if (request.IncludeStates)
            {
                tasks.Add(Task.Run(async () =>
                {
                    states = await _elasticsearchService.SearchStatesByNameAsync(
                        request.SearchTerm, request.Page, request.PageSize);
                }, cancellationToken));
            }

            if (request.IncludeMesoregions)
            {
                tasks.Add(Task.Run(async () =>
                {
                    mesoregions = await _elasticsearchService.SearchMesoregionsByNameAsync(
                        request.SearchTerm, request.Page, request.PageSize);
                }, cancellationToken));
            }

            if (request.IncludeMicroRegions)
            {
                tasks.Add(Task.Run(async () =>
                {
                    microRegions = await _elasticsearchService.SearchMicroRegionsByNameAsync(
                        request.SearchTerm, request.Page, request.PageSize);
                }, cancellationToken));
            }

            if (request.IncludeMunicipalities)
            {
                tasks.Add(Task.Run(async () =>
                {
                    municipalities = await _elasticsearchService.SearchMunicipalitiesByNameAsync(
                        request.SearchTerm, request.Page, request.PageSize);
                }, cancellationToken));
            }

            if (request.IncludeDistricts)
            {
                tasks.Add(Task.Run(async () =>
                {
                    districts = await _elasticsearchService.SearchDistrictsByNameAsync(
                        request.SearchTerm, request.Page, request.PageSize);
                }, cancellationToken));
            }

            if (request.IncludeSubDistricts)
            {
                tasks.Add(Task.Run(async () =>
                {
                    subDistricts = await _elasticsearchService.SearchSubDistrictsByNameAsync(
                        request.SearchTerm, request.Page, request.PageSize);
                }, cancellationToken));
            }

            await Task.WhenAll(tasks);

            _logger.LogInformation("Busca concluída para o termo '{SearchTerm}'. " +
                                   "Regiões: {RegionsCount}, Estados: {StatesCount}, " +
                                   "Mesorregiões: {MesoregionsCount}, Microrregiões: {MicroRegionsCount}, " +
                                   "Municípios: {MunicipalitiesCount}, Distritos: {DistrictsCount}, " +
                                   "Subdistritos: {SubDistrictsCount}",
                request.SearchTerm, regions.Count(), states.Count(), mesoregions.Count(),
                microRegions.Count(), municipalities.Count(), districts.Count(), subDistricts.Count());

            return new SearchLocationsResult(
                regions,
                states,
                mesoregions,
                microRegions,
                municipalities,
                districts,
                subDistricts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar localizações com o termo '{SearchTerm}'", request.SearchTerm);
            throw;
        }
    }
}
