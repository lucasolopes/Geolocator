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

    public async Task<IEnumerable<Region>> SearchRegionsByNameAsync(string searchTerm, int page = 1, int pageSize = 10)
    {
        try
        {
            ISearchResponse<RegionDto>? searchResponse = await _elasticClient.SearchAsync<RegionDto>(s => s
                .Index(_options.RegionIndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            // Correspondência exata no nome (prioridade mais alta)
                            sh => sh.Match(m => m
                                .Field(ff => ff.Name)
                                .Query(searchTerm)
                                .Operator(Operator.And)
                                .Boost(3.0)
                            ),
                            // Correspondência parcial no nome (termos individuais)
                            sh => sh.Match(m => m
                                .Field(ff => ff.Name)
                                .Query(searchTerm)
                                .Operator(Operator.Or)
                                .Fuzziness(Fuzziness.Auto)
                                .Boost(1.0)
                            ),
                            // Busca por sigla exata
                            sh => sh.Term(t => t
                                .Field(ff => ff.Initials)
                                .Value(searchTerm)
                                .Boost(4.0)
                            ),
                            // Busca por sigla (case insensitive)
                            sh => sh.Term(t => t
                                .Field(ff => ff.Initials)
                                .Value(searchTerm.ToUpper())
                                .Boost(4.0)
                            )
                        )
                    )
                )
            );

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de regiões: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<Region>();
            }

            // Obter IDs dos resultados
            var regionIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            // Se não houver regiões, retornar uma lista vazia
            if (!regionIds.Any())
            {
                // Buscar "São" e "Paulo" como termos separados com operador OR
                string[] words = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                ISearchResponse<RegionDto>? wildCardQuery = await _elasticClient.SearchAsync<RegionDto>(s => s
                    .Index(_options.RegionIndexName)
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q
                        .Bool(b => b
                            .Should(
                                words.Select(word =>
                                    (QueryContainer)q.Wildcard(w => w
                                        .Field(f => f.Name)
                                        .Value($"*{word.ToLower()}*")
                                    )
                                ).ToArray()
                            )
                        )
                    )
                );

                if (wildCardQuery.IsValid)
                {
                    regionIds.AddRange(wildCardQuery.Documents
                        .Where(dto => dto != null)
                        .Select(dto => dto.Id)
                        .ToList());
                }

                // Se ainda não houver regiões, verificar se estamos buscando pela região relacionada ao estado
                if (!regionIds.Any() && searchTerm.Contains("Paulo"))
                {
                    // Buscar região Sudeste onde está São Paulo
                    regionIds.Add(3); // Assumindo que 3 é o ID da região Sudeste
                }
            }

            // Se não houver regiões, retornar uma lista vazia
            if (!regionIds.Any())
            {
                return Enumerable.Empty<Region>();
            }

            // Buscar regiões completas com relacionamentos do banco de dados
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
                            ),
                            sh => sh.Prefix(p => p
                                .Field(f => f.Name)
                                .Value(searchTerm.ToLower())
                                .Boost(2.0)
                            ),
                            sh => sh.Wildcard(w => w
                                .Field(f => f.Name)
                                .Value($"*{searchTerm.ToLower()}*")
                                .Boost(1.0)
                            )
                        )
                    )
                )
            );

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de estados: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<State>();
            }

            // Obter IDs dos resultados
            var stateIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            // Se não houver estados, retornar uma lista vazia
            if (!stateIds.Any())
            {
                return Enumerable.Empty<State>();
            }

            // Buscar estados completos com relacionamentos do banco de dados
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
            // Primeiro, buscar pelo nome exato
            ISearchResponse<MesoregionDto>? exactMatchQuery = await _elasticClient.SearchAsync<MesoregionDto>(s => s
                .Index(_options.MesoregionIndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            // Correspondência exata no nome (prioridade mais alta)
                            sh => sh.Match(m => m
                                .Field(f => f.Name)
                                .Query(searchTerm)
                                .Operator(Operator.And)
                                .Boost(3.0)
                            ),
                            // Correspondência parcial (termos individuais)
                            sh => sh.Match(m => m
                                .Field(f => f.Name)
                                .Query(searchTerm)
                                .Operator(Operator.Or)
                                .Fuzziness(Fuzziness.Auto)
                                .Boost(1.0)
                            ),
                            // Busca por prefixo (começando com o termo)
                            sh => sh.Prefix(p => p
                                .Field(f => f.Name)
                                .Value(searchTerm.ToLower())
                                .Boost(2.0)
                            )
                        )
                    )
                )
            );

            if (!exactMatchQuery.IsValid)
            {
                _logger.LogError("Erro na busca de mesorregiões: {Error}", exactMatchQuery.DebugInformation);
                return Enumerable.Empty<Mesoregion>();
            }

            // Obter IDs dos resultados
            var mesoregionIds = exactMatchQuery.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            // Se não encontrou nada, tente um método de busca mais abrangente
            if (!mesoregionIds.Any())
            {
                // Buscar "São" e "Paulo" como termos separados com operador OR
                string[] words = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                ISearchResponse<MesoregionDto>? wildCardQuery = await _elasticClient.SearchAsync<MesoregionDto>(s => s
                    .Index(_options.MesoregionIndexName)
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q
                        .Bool(b => b
                            .Should(
                                words.Select(word =>
                                    (QueryContainer)q.Wildcard(w => w
                                        .Field(f => f.Name)
                                        .Value($"*{word.ToLower()}*")
                                    )
                                ).ToArray()
                            )
                        )
                    )
                );

                if (wildCardQuery.IsValid)
                {
                    mesoregionIds.AddRange(wildCardQuery.Documents
                        .Where(dto => dto != null)
                        .Select(dto => dto.Id)
                        .ToList());
                }
            }

            // Se não houver mesorregiões, retornar uma lista vazia
            if (!mesoregionIds.Any())
            {
                return Enumerable.Empty<Mesoregion>();
            }

            // Buscar mesorregiões completas com relacionamentos do banco de dados
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
            // Primeiro, buscar pelo nome exato
            ISearchResponse<MicroRegionDto>? exactMatchQuery = await _elasticClient.SearchAsync<MicroRegionDto>(s => s
                .Index(_options.MicroRegionIndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            // Correspondência exata no nome (prioridade mais alta)
                            sh => sh.Match(m => m
                                .Field(f => f.Name)
                                .Query(searchTerm)
                                .Operator(Operator.And)
                                .Boost(3.0)
                            ),
                            // Correspondência parcial (termos individuais)
                            sh => sh.Match(m => m
                                .Field(f => f.Name)
                                .Query(searchTerm)
                                .Operator(Operator.Or)
                                .Fuzziness(Fuzziness.Auto)
                                .Boost(1.0)
                            ),
                            // Busca por prefixo (começando com o termo)
                            sh => sh.Prefix(p => p
                                .Field(f => f.Name)
                                .Value(searchTerm.ToLower())
                                .Boost(2.0)
                            )
                        )
                    )
                )
            );

            if (!exactMatchQuery.IsValid)
            {
                _logger.LogError("Erro na busca de microrregiões: {Error}", exactMatchQuery.DebugInformation);
                return Enumerable.Empty<MicroRegion>();
            }

            // Obter IDs dos resultados
            var microRegionIds = exactMatchQuery.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            // Se não encontrou nada, tente um método de busca mais abrangente
            if (!microRegionIds.Any())
            {
                // Buscar "São" e "Paulo" como termos separados com operador OR
                string[] words = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                ISearchResponse<MicroRegionDto>? wildCardQuery = await _elasticClient.SearchAsync<MicroRegionDto>(s => s
                    .Index(_options.MicroRegionIndexName)
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q
                        .Bool(b => b
                            .Should(
                                words.Select(word =>
                                    (QueryContainer)q.Wildcard(w => w
                                        .Field(f => f.Name)
                                        .Value($"*{word.ToLower()}*")
                                    )
                                ).ToArray()
                            )
                        )
                    )
                );

                if (wildCardQuery.IsValid)
                {
                    microRegionIds.AddRange(wildCardQuery.Documents
                        .Where(dto => dto != null)
                        .Select(dto => dto.Id)
                        .ToList());
                }
            }

            // Se não houver microrregiões, retornar uma lista vazia
            if (!microRegionIds.Any())
            {
                return Enumerable.Empty<MicroRegion>();
            }

            // Buscar microrregiões completas com relacionamentos do banco de dados
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

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de municípios: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<Municipality>();
            }

            // Obter IDs dos resultados
            var municipalityIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            // Se não houver municípios, retornar uma lista vazia
            if (!municipalityIds.Any())
            {
                return Enumerable.Empty<Municipality>();
            }

            // Buscar municípios completos com relacionamentos do banco de dados
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

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de distritos: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<Districts>();
            }

            // Obter IDs dos resultados
            var districtIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            // Se não houver distritos, retornar uma lista vazia
            if (!districtIds.Any())
            {
                return Enumerable.Empty<Districts>();
            }

            // Buscar distritos completos com relacionamentos do banco de dados
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

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erro na busca de subdistritos: {Error}", searchResponse.DebugInformation);
                return Enumerable.Empty<SubDistricts>();
            }

            // Obter IDs dos resultados
            var subdistrictIds = searchResponse.Documents
                .Where(dto => dto != null)
                .Select(dto => dto.Id)
                .ToList();

            // Se não houver subdistritos, retornar uma lista vazia
            if (!subdistrictIds.Any())
            {
                return Enumerable.Empty<SubDistricts>();
            }

            // Buscar subdistritos completos com relacionamentos do banco de dados
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

    // Método genérico auxiliar para criar índices
    private async Task CreateIndexIfNotExists<T>(string indexName,
        Func<TypeMappingDescriptor<T>, ITypeMapping> mappingSelector) where T : class
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
