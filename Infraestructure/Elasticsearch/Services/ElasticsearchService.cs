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
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticsearchService> _logger;
    private readonly ElasticsearchOptions _options;

    public ElasticsearchService(
        IElasticClient elasticClient,
        IOptions<ElasticsearchOptions> options,
        ILogger<ElasticsearchService> logger)
    {
        _elasticClient = elasticClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task CreateIndicesIfNotExistAsync()
    {
        _logger.LogInformation("Verificando se os índices do Elasticsearch existem");

        await CreateIndexIfNotExists<RegionDto>(_options.RegionIndexName, r => r
            .Properties(p => p
                .Text(t => t.Name(n => n.Name).Analyzer("brazilian"))
                .Keyword(k => k.Name(n => n.Initials))
            ));

        await CreateIndexIfNotExists<StateDto>(_options.StateIndexName, r => r
            .Properties(p => p
                .Text(t => t.Name(n => n.Name).Analyzer("brazilian"))
                .Keyword(k => k.Name(n => n.Initials))
                .Number(n => n.Name(n => n.RegionId).Type(NumberType.Integer))
            ));

        await CreateIndexIfNotExists<MesoregionDto>(_options.MesoregionIndexName, r => r
            .Properties(p => p
                .Text(t => t.Name(n => n.Name).Analyzer("brazilian"))
                .Number(n => n.Name(n => n.StateId).Type(NumberType.Integer))
            ));

        await CreateIndexIfNotExists<MicroRegionDto>(_options.MicroRegionIndexName, r => r
            .Properties(p => p
                .Text(t => t.Name(n => n.Name).Analyzer("brazilian"))
                .Number(n => n.Name(n => n.MesoregionId).Type(NumberType.Integer))
            ));

        await CreateIndexIfNotExists<MunicipalityDto>(_options.MunicipalityIndexName, r => r
            .Properties(p => p
                .Text(t => t.Name(n => n.Name).Analyzer("brazilian"))
                .Number(n => n.Name(n => n.MicroRegionId).Type(NumberType.Integer))
            ));

        await CreateIndexIfNotExists<DistrictsDto>(_options.DistrictIndexName, r => r
            .Properties(p => p
                .Text(t => t.Name(n => n.Name).Analyzer("brazilian"))
                .Number(n => n.Name(n => n.MunicipalityId).Type(NumberType.Integer))
            ));

        await CreateIndexIfNotExists<SubDistrictsDto>(_options.SubDistrictIndexName, r => r
            .Properties(p => p
                .Text(t => t.Name(n => n.Name).Analyzer("brazilian"))
                .Number(n => n.Name(n => n.DistrictId).Type(NumberType.Integer))
            ));

        _logger.LogInformation("Verificação e criação de índices concluída");
    }

    // Implementação dos métodos de indexação
    public async Task<bool> IndexRegionsAsync(IEnumerable<Region> regions)
    {
        try
        {
            // Convertendo para DTOs
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
            // Convertendo para DTOs
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
            // Convertendo para DTOs
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
            // Convertendo para DTOs
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
            // Convertendo para DTOs
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
            // Convertendo para DTOs
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
            // Convertendo para DTOs
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

    // Implementação dos métodos de busca
    public async Task<IEnumerable<Region>> SearchRegionsByNameAsync(string searchTerm, int page = 1, int pageSize = 10)
    {
        try
        {
            ISearchResponse<RegionDto>? searchResponse = await _elasticClient.SearchAsync<RegionDto>(s => s
                .Index(_options.RegionIndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f
                            .Field(ff => ff.Name, 2.0)
                            .Field(ff => ff.Initials)
                        )
                        .Query(searchTerm)
                        .Type(TextQueryType.BestFields)
                        .Fuzziness(Fuzziness.Auto)
                    )
                )
            );

            return MapSearchResults<Region, RegionDto>(
                searchResponse,
                dto => new Region(dto.Id, dto.Name, dto.Initials)
            );
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
            ISearchResponse<StateDto>? searchResponse = await _elasticClient.SearchAsync<StateDto>(s => s
                .Index(_options.StateIndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            // Busca por nome
                            sh => sh.Match(m => m
                                .Field(f => f.Name)
                                .Query(searchTerm)
                                .Fuzziness(Fuzziness.Auto)
                                .Boost(1.0)
                            ),
                            // Busca por sigla exata (case sensitive)
                            sh => sh.Term(t => t
                                .Field(f => f.Initials)
                                .Value(searchTerm)
                                .Boost(2.0)
                            ),
                            // Busca por sigla (case insensitive)
                            sh => sh.Term(t => t
                                .Field(f => f.Initials)
                                .Value(searchTerm.ToUpper())
                                .Boost(2.0)
                            ),
                            // Busca por sigla minúscula
                            sh => sh.Term(t => t
                                .Field(f => f.Initials)
                                .Value(searchTerm.ToLower())
                                .Boost(2.0)
                            )
                        )
                    )
                )
            );

            return MapSearchResults<State, StateDto>(
                searchResponse,
                dto => new State(dto.Id, dto.Name, dto.Initials, dto.RegionId)
            );
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
            ISearchResponse<MesoregionDto>? searchResponse = await _elasticClient.SearchAsync<MesoregionDto>(s => s
                .Index(_options.MesoregionIndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(searchTerm)
                        .Fuzziness(Fuzziness.Auto)
                    )
                )
            );

            return MapSearchResults<Mesoregion, MesoregionDto>(
                searchResponse,
                dto => new Mesoregion(dto.Id, dto.Name, dto.StateId)
            );
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
            ISearchResponse<MicroRegionDto>? searchResponse = await _elasticClient.SearchAsync<MicroRegionDto>(s => s
                .Index(_options.MicroRegionIndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(searchTerm)
                        .Fuzziness(Fuzziness.Auto)
                    )
                )
            );

            return MapSearchResults<MicroRegion, MicroRegionDto>(
                searchResponse,
                dto => new MicroRegion(dto.Id, dto.Name, dto.MesoregionId)
            );
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
            ISearchResponse<MunicipalityDto>? searchResponse = await _elasticClient.SearchAsync<MunicipalityDto>(s => s
                .Index(_options.MunicipalityIndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(searchTerm)
                        .Fuzziness(Fuzziness.Auto)
                    )
                )
            );

            return MapSearchResults<Municipality, MunicipalityDto>(
                searchResponse,
                dto => new Municipality(dto.Id, dto.Name, dto.MicroRegionId)
            );
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
            ISearchResponse<DistrictsDto>? searchResponse = await _elasticClient.SearchAsync<DistrictsDto>(s => s
                .Index(_options.DistrictIndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(searchTerm)
                        .Fuzziness(Fuzziness.Auto)
                    )
                )
            );

            return MapSearchResults<Districts, DistrictsDto>(
                searchResponse,
                dto => new Districts(dto.Id, dto.Name, dto.MunicipalityId)
            );
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
            ISearchResponse<SubDistrictsDto>? searchResponse = await _elasticClient.SearchAsync<SubDistrictsDto>(s => s
                .Index(_options.SubDistrictIndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(searchTerm)
                        .Fuzziness(Fuzziness.Auto)
                    )
                )
            );

            return MapSearchResults<SubDistricts, SubDistrictsDto>(
                searchResponse,
                dto => new SubDistricts(dto.Id, dto.Name, dto.DistrictId)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar subdistritos: {Message}", ex.Message);
            return Enumerable.Empty<SubDistricts>();
        }
    }

    // Método genérico para mapear resultados
    private IEnumerable<TEntity> MapSearchResults<TEntity, TDto>(ISearchResponse<TDto> response,
        Func<TDto, TEntity> mapper)
        where TEntity : class
        where TDto : class
    {
        if (!response.IsValid)
        {
            _logger.LogError("Erro na busca: {Error}", response.DebugInformation);
            return Enumerable.Empty<TEntity>();
        }

        return response.Documents
            .Where(dto => dto != null)
            .Select(mapper);
    }

    private async Task CreateIndexIfNotExists<T>(string indexName,
        Func<TypeMappingDescriptor<T>, ITypeMapping> mappingSelector) where T : class
    {
        ExistsResponse indexExists = await _elasticClient.Indices.ExistsAsync(indexName);
        if (!indexExists.Exists)
        {
            _logger.LogInformation("Criando índice {IndexName}", indexName);

            CreateIndexResponse? createIndexResponse = await _elasticClient.Indices.CreateAsync(indexName, c => c
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(aa => aa
                            .Custom("brazilian", ca => ca
                                .Tokenizer("standard")
                                .Filters("lowercase", "brazilian_stemmer", "asciifolding")
                            )
                        )
                        .TokenFilters(tf => tf
                            .Stemmer("brazilian_stemmer", st => st
                                .Language("brazilian")
                            )
                        )
                    )
                )
                .Map(mappingSelector)
            );

            if (!createIndexResponse.IsValid)
            {
                _logger.LogError("Erro ao criar índice {IndexName}: {Error}",
                    indexName, createIndexResponse.DebugInformation);
                throw new Exception($"Falha ao criar índice {indexName}: {createIndexResponse.DebugInformation}");
            }
        }
    }
}
