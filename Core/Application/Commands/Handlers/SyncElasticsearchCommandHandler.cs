using Application.Commands.ElasticsearchSync;
using Application.Interfaces.Repositories;
using Application.Interfaces.Search;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Handlers;

public class SyncElasticsearchCommandHandler(
    IElasticsearchService elasticsearchService,
    IRegionRepository regionRepository,
    IStateRepository stateRepository,
    IMesoregionRepository mesoregionRepository,
    IMicroRegionRepository microRegionRepository,
    IMunicipalityRepository municipalityRepository,
    IDistrictsRepository districtsRepository,
    ISubDistrictsRepository subDistrictsRepository,
    ILogger<SyncElasticsearchCommandHandler> logger)
    : IRequestHandler<SyncElasticsearchCommand, SyncElasticsearchResult>
{
    private readonly IDistrictsRepository _districtsRepository = districtsRepository;
    private readonly IElasticsearchService _elasticsearchService = elasticsearchService;
    private readonly ILogger<SyncElasticsearchCommandHandler> _logger = logger;
    private readonly IMesoregionRepository _mesoregionRepository = mesoregionRepository;
    private readonly IMicroRegionRepository _microRegionRepository = microRegionRepository;
    private readonly IMunicipalityRepository _municipalityRepository = municipalityRepository;
    private readonly IRegionRepository _regionRepository = regionRepository;
    private readonly IStateRepository _stateRepository = stateRepository;
    private readonly ISubDistrictsRepository _subDistrictsRepository = subDistrictsRepository;

    public async Task<SyncElasticsearchResult> Handle(SyncElasticsearchCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando sincronização manual com o Elasticsearch");

        try
        {
            await _elasticsearchService.CreateIndicesIfNotExistAsync();

            _logger.LogInformation("Sincronizando regiões com o Elasticsearch");
            List<Region> regions = await _regionRepository.GetAllAsync();
            bool regionsResult = await _elasticsearchService.IndexRegionsAsync(regions);

            _logger.LogInformation("Sincronizando estados com o Elasticsearch");
            List<State> states = await _stateRepository.GetAllAsync();
            bool statesResult = await _elasticsearchService.IndexStatesAsync(states);

            _logger.LogInformation("Sincronizando mesorregiões com o Elasticsearch");
            List<Mesoregion> mesoregions = await _mesoregionRepository.GetAllAsync();
            bool mesoregionsResult = await _elasticsearchService.IndexMesoregionsAsync(mesoregions);

            _logger.LogInformation("Sincronizando microrregiões com o Elasticsearch");
            List<MicroRegion> microRegions = await _microRegionRepository.GetAllAsync();
            bool microRegionsResult = await _elasticsearchService.IndexMicroRegionsAsync(microRegions);

            _logger.LogInformation("Sincronizando municípios com o Elasticsearch");
            List<Municipality> municipalities = await _municipalityRepository.GetAllAsync();
            bool municipalitiesResult = await _elasticsearchService.IndexMunicipalitiesAsync(municipalities);

            _logger.LogInformation("Sincronizando distritos com o Elasticsearch");
            List<Districts> districts = await _districtsRepository.GetAllAsync();
            bool districtsResult = await _elasticsearchService.IndexDistrictsAsync(districts);

            _logger.LogInformation("Sincronizando subdistritos com o Elasticsearch");
            List<SubDistricts> subDistricts = await _subDistrictsRepository.GetAllAsync();
            bool subDistrictsResult = await _elasticsearchService.IndexSubDistrictsAsync(subDistricts);

            bool success = regionsResult && statesResult && mesoregionsResult &&
                           microRegionsResult && municipalitiesResult &&
                           districtsResult && subDistrictsResult;

            _logger.LogInformation("Sincronização com o Elasticsearch concluída. Resultado: {Result}",
                success ? "Sucesso" : "Falha parcial");

            return new SyncElasticsearchResult(
                success,
                regionsResult,
                statesResult,
                mesoregionsResult,
                microRegionsResult,
                municipalitiesResult,
                districtsResult,
                subDistrictsResult
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização com o Elasticsearch: {Message}", ex.Message);

            return new SyncElasticsearchResult(
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                ex.Message
            );
        }
    }
}
