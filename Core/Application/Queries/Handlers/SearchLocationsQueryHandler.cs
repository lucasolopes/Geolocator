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

            // Executa as buscas SEQUENCIALMENTE para evitar problemas de concorrência
            if (request.IncludeRegions)
            {
                regions = await _elasticsearchService.SearchRegionsByNameAsync(
                    request.SearchTerm, request.Page, request.PageSize);
            }

            if (request.IncludeStates)
            {
                states = await _elasticsearchService.SearchStatesByNameAsync(
                    request.SearchTerm, request.Page, request.PageSize);
            }

            if (request.IncludeMesoregions)
            {
                mesoregions = await _elasticsearchService.SearchMesoregionsByNameAsync(
                    request.SearchTerm, request.Page, request.PageSize);
            }

            if (request.IncludeMicroRegions)
            {
                microRegions = await _elasticsearchService.SearchMicroRegionsByNameAsync(
                    request.SearchTerm, request.Page, request.PageSize);
            }

            if (request.IncludeMunicipalities)
            {
                municipalities = await _elasticsearchService.SearchMunicipalitiesByNameAsync(
                    request.SearchTerm, request.Page, request.PageSize);
            }

            if (request.IncludeDistricts)
            {
                districts = await _elasticsearchService.SearchDistrictsByNameAsync(
                    request.SearchTerm, request.Page, request.PageSize);
            }

            if (request.IncludeSubDistricts)
            {
                subDistricts = await _elasticsearchService.SearchSubDistrictsByNameAsync(
                    request.SearchTerm, request.Page, request.PageSize);
            }

            _logger.LogInformation("Busca concluída para o termo '{SearchTerm}'...", request.SearchTerm);

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
