using Application.Interfaces.Repositories;
using Application.Interfaces.Search;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BackgroundJobs.Jobs;

[DisallowConcurrentExecution]
public class ElasticsearchSyncJob : IJob
{
    private readonly IDistrictsRepository _districtsRepository;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<ElasticsearchSyncJob> _logger;
    private readonly IMesoregionRepository _mesoregionRepository;
    private readonly IMicroRegionRepository _microRegionRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IStateRepository _stateRepository;
    private readonly ISubDistrictsRepository _subDistrictsRepository;

    public ElasticsearchSyncJob(
        IElasticsearchService elasticsearchService,
        IRegionRepository regionRepository,
        IStateRepository stateRepository,
        IMesoregionRepository mesoregionRepository,
        IMicroRegionRepository microRegionRepository,
        IMunicipalityRepository municipalityRepository,
        IDistrictsRepository districtsRepository,
        ISubDistrictsRepository subDistrictsRepository,
        ILogger<ElasticsearchSyncJob> logger)
    {
        _elasticsearchService = elasticsearchService;
        _regionRepository = regionRepository;
        _stateRepository = stateRepository;
        _mesoregionRepository = mesoregionRepository;
        _microRegionRepository = microRegionRepository;
        _municipalityRepository = municipalityRepository;
        _districtsRepository = districtsRepository;
        _subDistrictsRepository = subDistrictsRepository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Iniciando sincronização de dados com o Elasticsearch");

        try
        {
            await _elasticsearchService.CreateIndicesIfNotExistAsync();

            _logger.LogInformation("Sincronizando regiões com o Elasticsearch");
            List<Region> regions = await _regionRepository.GetAllAsync();
            bool regionsResult = await _elasticsearchService.IndexRegionsAsync(regions);
            _logger.LogInformation("Sincronização de regiões com o Elasticsearch: {Result}",
                regionsResult ? "Sucesso" : "Falha");

            _logger.LogInformation("Sincronizando estados com o Elasticsearch");
            List<State> states = await _stateRepository.GetAllAsync();
            bool statesResult = await _elasticsearchService.IndexStatesAsync(states);
            _logger.LogInformation("Sincronização de estados com o Elasticsearch: {Result}",
                statesResult ? "Sucesso" : "Falha");

            _logger.LogInformation("Sincronizando mesorregiões com o Elasticsearch");
            List<Mesoregion> mesoregions = await _mesoregionRepository.GetAllAsync();
            bool mesoregionsResult = await _elasticsearchService.IndexMesoregionsAsync(mesoregions);
            _logger.LogInformation("Sincronização de mesorregiões com o Elasticsearch: {Result}",
                mesoregionsResult ? "Sucesso" : "Falha");

            _logger.LogInformation("Sincronizando microrregiões com o Elasticsearch");
            List<MicroRegion> microRegions = await _microRegionRepository.GetAllAsync();
            bool microRegionsResult = await _elasticsearchService.IndexMicroRegionsAsync(microRegions);
            _logger.LogInformation("Sincronização de microrregiões com o Elasticsearch: {Result}",
                microRegionsResult ? "Sucesso" : "Falha");

            _logger.LogInformation("Sincronizando municípios com o Elasticsearch");
            List<Municipality> municipalities = await _municipalityRepository.GetAllAsync();
            bool municipalitiesResult = await _elasticsearchService.IndexMunicipalitiesAsync(municipalities);
            _logger.LogInformation("Sincronização de municípios com o Elasticsearch: {Result}",
                municipalitiesResult ? "Sucesso" : "Falha");

            _logger.LogInformation("Sincronizando distritos com o Elasticsearch");
            List<Districts> districts = await _districtsRepository.GetAllAsync();
            bool districtsResult = await _elasticsearchService.IndexDistrictsAsync(districts);
            _logger.LogInformation("Sincronização de distritos com o Elasticsearch: {Result}",
                districtsResult ? "Sucesso" : "Falha");

            _logger.LogInformation("Sincronizando subdistritos com o Elasticsearch");
            List<SubDistricts> subDistricts = await _subDistrictsRepository.GetAllAsync();
            bool subDistrictsResult = await _elasticsearchService.IndexSubDistrictsAsync(subDistricts);
            _logger.LogInformation("Sincronização de subdistritos com o Elasticsearch: {Result}",
                subDistrictsResult ? "Sucesso" : "Falha");

            _logger.LogInformation("Sincronização com o Elasticsearch concluída");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização com o Elasticsearch: {Message}", ex.Message);
            throw;
        }
    }
}
