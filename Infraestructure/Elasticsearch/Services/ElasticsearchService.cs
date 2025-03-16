using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Search;
using Domain.Entities;
using Elasticsearch.DTOs;
using Elasticsearch.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;

namespace Elasticsearch.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IDistrictsRepository _districtsRepository;
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticsearchService> _logger;
    private readonly IMesoregionRepository _mesoregionRepository;
    private readonly IMicroRegionRepository _microRegionRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly ElasticsearchOptions _options;
    private readonly IRegionRepository _regionRepository;
    private readonly IStateRepository _stateRepository;
    private readonly ISubDistrictsRepository _subDistrictsRepository;

    public ElasticsearchService(
        IElasticClient elasticClient,
        IOptions<ElasticsearchOptions> options,
        ILogger<ElasticsearchService> logger,
        IStateRepository stateRepository,
        IRegionRepository regionRepository,
        IMesoregionRepository mesoregionRepository,
        IMicroRegionRepository microRegionRepository,
        IMunicipalityRepository municipalityRepository,
        IDistrictsRepository districtsRepository,
        ISubDistrictsRepository subDistrictsRepository)
    {
        _elasticClient = elasticClient;
        _options = options.Value;
        _logger = logger;
        _stateRepository = stateRepository;
        _regionRepository = regionRepository;
        _mesoregionRepository = mesoregionRepository;
        _microRegionRepository = microRegionRepository;
        _municipalityRepository = municipalityRepository;
        _districtsRepository = districtsRepository;
        _subDistrictsRepository = subDistrictsRepository;
    }

    public async Task CreateIndicesIfNotExistAsync()
    {
        _logger.LogInformation("Verificando se os índices do Elasticsearch existem");

        await CreateIndexIfNotExists<RegionDto>(_options.RegionIndexName);
        await CreateIndexIfNotExists<StateDto>(_options.StateIndexName);
        await CreateIndexIfNotExists<MesoregionDto>(_options.MesoregionIndexName);
        await CreateIndexIfNotExists<MicroRegionDto>(_options.MicroRegionIndexName);
        await CreateIndexIfNotExists<MunicipalityDto>(_options.MunicipalityIndexName);
        await CreateIndexIfNotExists<DistrictsDto>(_options.DistrictIndexName);
        await CreateIndexIfNotExists<SubDistrictsDto>(_options.SubDistrictIndexName);

        _logger.LogInformation("Verificação e criação de índices concluída");
    }

    public async Task<bool> IndexRegionsAsync(IEnumerable<Region> regions)
    {
        try
        {
            IEnumerable<RegionDto> regionDtos = regions.Select(r => new RegionDto
            {
                Id = r.Id,
                Name = r.Name,
                Initials = r.Initials
            });

            BulkResponse? bulkResponse = await _elasticClient.BulkAsync(b => b
                .Index(_options.RegionIndexName)
                .IndexMany(regionDtos)
            );

            if (bulkResponse.Errors)
            {
                _logger.LogError("Erro ao indexar regiões: {Error}", bulkResponse.DebugInformation);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao indexar regiões");
            return false;
        }
    }

    public async Task<bool> IndexStatesAsync(IEnumerable<State> states)
    {
        try
        {
            IEnumerable<StateDto> stateDtos = states.Select(s => new StateDto
            {
                Id = s.Id,
                Name = s.Name,
                Initials = s.Initials,
                RegionId = s.RegionId
            });

            BulkResponse? bulkResponse = await _elasticClient.BulkAsync(b => b
                .Index(_options.StateIndexName)
                .IndexMany(stateDtos)
            );

            if (bulkResponse.Errors)
            {
                _logger.LogError("Erro ao indexar estados: {Error}", bulkResponse.DebugInformation);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao indexar estados");
            return false;
        }
    }

    public async Task<bool> IndexMesoregionsAsync(IEnumerable<Mesoregion> mesoregions)
    {
        try
        {
            IEnumerable<MesoregionDto> mesoregionDtos = mesoregions.Select(m => new MesoregionDto
            {
                Id = m.Id,
                Name = m.Name,
                StateId = m.StateId
            });

            BulkResponse? bulkResponse = await _elasticClient.BulkAsync(b => b
                .Index(_options.MesoregionIndexName)
                .IndexMany(mesoregionDtos)
            );

            if (bulkResponse.Errors)
            {
                _logger.LogError("Erro ao indexar mesorregiões: {Error}", bulkResponse.DebugInformation);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao indexar mesorregiões");
            return false;
        }
    }

    public async Task<bool> IndexMicroRegionsAsync(IEnumerable<MicroRegion> microRegions)
    {
        try
        {
            IEnumerable<MicroRegionDto> microRegionDtos = microRegions.Select(m => new MicroRegionDto
            {
                Id = m.Id,
                Name = m.Name,
                MesoregionId = m.MesoregionId
            });

            BulkResponse? bulkResponse = await _elasticClient.BulkAsync(b => b
                .Index(_options.MicroRegionIndexName)
                .IndexMany(microRegionDtos)
            );

            if (bulkResponse.Errors)
            {
                _logger.LogError("Erro ao indexar microrregiões: {Error}", bulkResponse.DebugInformation);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao indexar microrregiões");
            return false;
        }
    }

    public async Task<bool> IndexMunicipalitiesAsync(IEnumerable<Municipality> municipalities)
    {
        try
        {
            IEnumerable<MunicipalityDto> municipalityDtos = municipalities.Select(m => new MunicipalityDto
            {
                Id = m.Id,
                Name = m.Name,
                MicroRegionId = m.MicroRegionId
            });

            BulkResponse? bulkResponse = await _elasticClient.BulkAsync(b => b
                .Index(_options.MunicipalityIndexName)
                .IndexMany(municipalityDtos)
            );

            if (bulkResponse.Errors)
            {
                _logger.LogError("Erro ao indexar municípios: {Error}", bulkResponse.DebugInformation);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao indexar municípios");
            return false;
        }
    }

    public async Task<bool> IndexDistrictsAsync(IEnumerable<Districts> districts)
    {
        try
        {
            IEnumerable<DistrictsDto> districtDtos = districts.Select(d => new DistrictsDto
            {
                Id = d.Id,
                Name = d.Name,
                MunicipalityId = d.MunicipalityId
            });

            BulkResponse? bulkResponse = await _elasticClient.BulkAsync(b => b
                .Index(_options.DistrictIndexName)
                .IndexMany(districtDtos)
            );

            if (bulkResponse.Errors)
            {
                _logger.LogError("Erro ao indexar distritos: {Error}", bulkResponse.DebugInformation);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao indexar distritos");
            return false;
        }
    }

    public async Task<bool> IndexSubDistrictsAsync(IEnumerable<SubDistricts> subDistricts)
    {
        try
        {
            IEnumerable<SubDistrictsDto> subDistrictDtos = subDistricts.Select(s => new SubDistrictsDto
            {
                Id = s.Id,
                Name = s.Name,
                DistrictId = s.DistrictId
            });

            BulkResponse? bulkResponse = await _elasticClient.BulkAsync(b => b
                .Index(_options.SubDistrictIndexName)
                .IndexMany(subDistrictDtos)
            );

            if (bulkResponse.Errors)
            {
                _logger.LogError("Erro ao indexar subdistritos: {Error}", bulkResponse.DebugInformation);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao indexar subdistritos");
            return false;
        }
    }

    #region Index Creation Utilities

    private async Task CreateIndexIfNotExists<T>(string indexName) where T : class
    {
        ExistsResponse indexExists = await _elasticClient.Indices.ExistsAsync(indexName);
        if (!indexExists.Exists)
        {
            _logger.LogInformation("Criando índice {IndexName}", indexName);

            CreateIndexResponse? createIndexResponse = await _elasticClient.Indices.CreateAsync(indexName, c => c
                .Settings(s => s
                    .NumberOfShards(1)
                    .NumberOfReplicas(0)
                    .RefreshInterval("30s")
                    .Setting("index.max_ngram_diff", 13)
                    .Analysis(a => a
                        .Analyzers(aa => aa
                            .Custom("brazilian", ca => ca
                                .Tokenizer("standard")
                                .Filters("lowercase", "brazilian_stemmer", "asciifolding", "ngram_filter")
                            )
                            .Custom("edge_ngram_analyzer", ca => ca
                                .Tokenizer("edge_ngram_tokenizer")
                                .Filters("lowercase", "asciifolding")
                            )
                        )
                        .Tokenizers(t => t
                            .EdgeNGram("edge_ngram_tokenizer", e => e
                                .MinGram(2)
                                .MaxGram(15)
                                .TokenChars(TokenChar.Letter, TokenChar.Digit)
                            )
                        )
                        .TokenFilters(tf => tf
                            .Stemmer("brazilian_stemmer", st => st
                                .Language("brazilian")
                            )
                            .NGram("ngram_filter", ng => ng
                                .MinGram(2)
                                .MaxGram(10)
                            )
                            .Synonym("synonym_filter", sf => sf
                                .Synonyms(
                                    "sp, são paulo",
                                    "rj, rio de janeiro",
                                    "df, brasília, distrito federal",
                                    "ba, bahia",
                                    "rs, rio grande do sul",
                                    "mg, minas gerais"
                                )
                            )
                        )
                    )
                )
                .Map<T>(m => m
                    .Properties(p => p
                        .Text(t => t
                            .Name("name")
                            .Analyzer("brazilian")
                            .Fields(f => f
                                .Text(ft => ft.Name("edge").Analyzer("edge_ngram_analyzer"))
                                .Keyword(k => k.Name("keyword").IgnoreAbove(256))
                                .Completion(c => c.Name("suggest"))
                            )
                        )
                    )
                )
            );

            if (!createIndexResponse.IsValid)
            {
                _logger.LogError("Erro ao criar índice {IndexName}: {Error}",
                    indexName, createIndexResponse.DebugInformation);
                throw new Exception($"Falha ao criar índice {indexName}: {createIndexResponse.DebugInformation}");
            }
        }
    }

    #endregion

    #region Search Methods with Advanced Query

    public async Task<IEnumerable<Region>> SearchRegionsByNameAsync(string searchTerm, int page = 1, int pageSize = 10)
    {
        try
        {
            ISearchResponse<RegionDto> searchResponse = await PerformAdvancedSearch<RegionDto>(
                _options.RegionIndexName,
                searchTerm,
                page,
                pageSize);

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de regiões: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<Region>();
            }


            var regionIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            if (!regionIds.Any())
            {
                return Enumerable.Empty<Region>();
            }

            List<Region> regions = await _regionRepository.GetByIdsWithRelationshipsAsync(regionIds);
            return regions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar regiões: {Message}", ex.Message);
            return Enumerable.Empty<Region>();
        }
    }

    public async Task<IEnumerable<State>> SearchStatesByNameAsync(string searchTerm, int page = 1, int pageSize = 10)
    {
        try
        {
            ISearchResponse<StateDto> searchResponse = await PerformAdvancedSearch<StateDto>(
                _options.StateIndexName,
                searchTerm,
                page,
                pageSize);

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de estados: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<State>();
            }


            var stateIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            if (!stateIds.Any())
            {
                return Enumerable.Empty<State>();
            }

            List<State> states = await _stateRepository.GetByIdsWithRelationshipsAsync(stateIds);
            return states;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estados: {Message}", ex.Message);
            return Enumerable.Empty<State>();
        }
    }

    public async Task<IEnumerable<Mesoregion>> SearchMesoregionsByNameAsync(string searchTerm, int page = 1,
        int pageSize = 10)
    {
        try
        {
            ISearchResponse<MesoregionDto> searchResponse = await PerformAdvancedSearch<MesoregionDto>(
                _options.MesoregionIndexName,
                searchTerm,
                page,
                pageSize);

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de mesorregiões: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<Mesoregion>();
            }

            var mesoregionIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            if (!mesoregionIds.Any())
            {
                return Enumerable.Empty<Mesoregion>();
            }

            List<Mesoregion> mesoregions = await _mesoregionRepository.GetByIdsWithRelationshipsAsync(mesoregionIds);
            return mesoregions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar mesorregiões: {Message}", ex.Message);
            return Enumerable.Empty<Mesoregion>();
        }
    }

    public async Task<IEnumerable<MicroRegion>> SearchMicroRegionsByNameAsync(string searchTerm, int page = 1,
        int pageSize = 10)
    {
        try
        {
            ISearchResponse<MicroRegionDto> searchResponse = await PerformAdvancedSearch<MicroRegionDto>(
                _options.MicroRegionIndexName,
                searchTerm,
                page,
                pageSize);

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de microrregiões: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<MicroRegion>();
            }

            var microRegionIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            if (!microRegionIds.Any())
            {
                return Enumerable.Empty<MicroRegion>();
            }

            List<MicroRegion> microRegions =
                await _microRegionRepository.GetByIdsWithRelationshipsAsync(microRegionIds);
            return microRegions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar microrregiões: {Message}", ex.Message);
            return Enumerable.Empty<MicroRegion>();
        }
    }

    public async Task<IEnumerable<Municipality>> SearchMunicipalitiesByNameAsync(string searchTerm, int page = 1,
        int pageSize = 10)
    {
        try
        {
            ISearchResponse<MunicipalityDto> searchResponse = await PerformAdvancedSearch<MunicipalityDto>(
                _options.MunicipalityIndexName,
                searchTerm,
                page,
                pageSize);

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de municípios: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<Municipality>();
            }

            var municipalityIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            if (!municipalityIds.Any())
            {
                return Enumerable.Empty<Municipality>();
            }

            List<Municipality> municipalities =
                await _municipalityRepository.GetByIdsWithRelationshipsAsync(municipalityIds);
            return municipalities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar municípios: {Message}", ex.Message);
            return Enumerable.Empty<Municipality>();
        }
    }

    public async Task<IEnumerable<Districts>> SearchDistrictsByNameAsync(string searchTerm, int page = 1,
        int pageSize = 10)
    {
        try
        {
            ISearchResponse<DistrictsDto> searchResponse = await PerformAdvancedSearch<DistrictsDto>(
                _options.DistrictIndexName,
                searchTerm,
                page,
                pageSize);

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de distritos: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<Districts>();
            }

            var districtIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            if (!districtIds.Any())
            {
                return Enumerable.Empty<Districts>();
            }

            List<Districts> districts = await _districtsRepository.GetByIdsWithRelationshipsAsync(districtIds);
            return districts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar distritos: {Message}", ex.Message);
            return Enumerable.Empty<Districts>();
        }
    }

    public async Task<IEnumerable<SubDistricts>> SearchSubDistrictsByNameAsync(string searchTerm, int page = 1,
        int pageSize = 10)
    {
        try
        {
            ISearchResponse<SubDistrictsDto> searchResponse = await PerformAdvancedSearch<SubDistrictsDto>(
                _options.SubDistrictIndexName,
                searchTerm,
                page,
                pageSize);

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de subdistritos: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<SubDistricts>();
            }

            var subdistrictIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            if (!subdistrictIds.Any())
            {
                return Enumerable.Empty<SubDistricts>();
            }

            List<SubDistricts> subdistricts =
                await _subDistrictsRepository.GetByIdsWithRelationshipsAsync(subdistrictIds);
            return subdistricts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar subdistritos: {Message}", ex.Message);
            return Enumerable.Empty<SubDistricts>();
        }
    }

    #endregion

    #region Common Search Methods

    private async Task<ISearchResponse<T>> PerformAdvancedSearch<T>(string indexName, string searchTerm, int page = 1,
        int pageSize = 10) where T : class
    {
        searchTerm = NormalizeSearchTerm(searchTerm);
        IEnumerable<string> tokens = TokenizeSearchTerm(searchTerm);

        ISearchResponse<T>? searchResponse = await _elasticClient.SearchAsync<T>(s => s
            .Index(indexName)
            .From((page - 1) * pageSize)
            .Size(pageSize)
            .Query(q => BuildAdvancedQuery<T>(searchTerm, tokens))
            .Sort(ss => ss.Descending(SortSpecialField.Score))
        );

        _logger.LogDebug("Elasticsearch query: {Query}", searchResponse.DebugInformation);

        return searchResponse;
    }

    private QueryContainer BuildAdvancedQuery<T>(string searchTerm, IEnumerable<string> tokens) where T : class
    {
        const string fieldName = "name";

        return new BoolQuery
        {
            Should = new List<QueryContainer>
            {
                new MatchQuery
                {
                    Field = fieldName,
                    Query = searchTerm,
                    Boost = 10
                },

                new MatchQuery
                {
                    Field = fieldName,
                    Query = searchTerm,
                    Fuzziness = Fuzziness.EditDistance(2),
                    Boost = 5
                },

                new PrefixQuery
                {
                    Field = fieldName,
                    Value = searchTerm.ToLowerInvariant(),
                    Boost = 3
                },

                new BoolQuery
                {
                    Should = tokens.Select(token =>
                        (QueryContainer)new MatchQuery
                        {
                            Field = fieldName,
                            Query = token,
                            Boost = 2
                        }
                    ).ToList(),
                    Boost = 2
                },

                new WildcardQuery
                {
                    Field = fieldName,
                    Value = $"*{searchTerm.ToLowerInvariant()}*",
                    Boost = 1
                }
            },
            MinimumShouldMatch = 1
        };
    }

    private string NormalizeSearchTerm(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return searchTerm;
        }

        string normalized = searchTerm.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new();

        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC).Trim().ToLower();
    }

    private IEnumerable<string> TokenizeSearchTerm(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Enumerable.Empty<string>();
        }

        return Regex.Split(searchTerm, @"[\s\-_.,;:!?]+")
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .Select(token => token.Trim().ToLowerInvariant());
    }

    #endregion
}
