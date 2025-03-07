using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ExternalServices.HttpClients;

public class IbgeService : IIbgeService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<IbgeService> _logger;

    public IbgeService(HttpClient httpClient, ILogger<IbgeService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _httpClient.BaseAddress = new Uri("https://servicodados.ibge.gov.br/api/v1/");

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<IbgeRegionDto>> GetRegionsFromIbgeAsync()
    {
        try
        {
            List<IbgeRegionDto>? response = await _httpClient.GetFromJsonAsync<List<IbgeRegionDto>>(
                "localidades/regioes",
                _jsonOptions);

            return response ?? new List<IbgeRegionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar Regioes do IBGE");
            throw;
        }
    }

    public async Task<List<State>> GetStatesAsync()
    {
        try
        {
            List<IbgeStateDto>? response =
                await _httpClient.GetFromJsonAsync<List<IbgeStateDto>>("localidades/estados", _jsonOptions);

            if (response == null)
            {
                return new List<State>();
            }

            var states = new List<State>();
            foreach (IbgeStateDto item in response)
            {
                var state = new State(
                    item.Id,
                    item.Nome,
                    item.Sigla,
                    item.Regiao.Id
                );

                states.Add(state);
            }

            return states;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estados do IBGE");
            throw;
        }
    }

    public async Task<List<Mesoregion>> GetMesoregionsAsync(int stateId)
    {
        try
        {
            List<IbgeMesoregionDto>? response =
                await _httpClient.GetFromJsonAsync<List<IbgeMesoregionDto>>(
                    $"localidades/estados/{stateId}/mesorregioes", _jsonOptions);

            if (response == null)
            {
                return new List<Mesoregion>();
            }

            var mesoregions = new List<Mesoregion>();
            foreach (IbgeMesoregionDto item in response)
            {
                var mesoregion = new Mesoregion(
                    item.Id,
                    item.Nome,
                    stateId
                );

                mesoregions.Add(mesoregion);
            }

            return mesoregions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar mesorregiões do IBGE para o estado {StateId}", stateId);
            throw;
        }
    }

    public async Task<List<MicroRegion>> GetMicroregionsAsync(int mesoregionId)
    {
        try
        {
            List<IbgeMicroregionDto>? response =
                await _httpClient.GetFromJsonAsync<List<IbgeMicroregionDto>>(
                    $"localidades/mesorregioes/{mesoregionId}/microrregioes", _jsonOptions);

            if (response == null)
            {
                return new List<MicroRegion>();
            }

            var microregions = new List<MicroRegion>();
            foreach (IbgeMicroregionDto item in response)
            {
                var microregion = new MicroRegion(
                    item.Id,
                    item.Nome,
                    mesoregionId
                );

                microregions.Add(microregion);
            }

            return microregions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar microrregiões do IBGE para a mesorregião {MesoregionId}",
                mesoregionId);
            throw;
        }
    }

    public async Task<List<Municipality>> GetMunicipalitiesAsync(int microregionId)
    {
        try
        {
            List<IbgeMunicipalityDto>? response =
                await _httpClient.GetFromJsonAsync<List<IbgeMunicipalityDto>>(
                    $"localidades/microrregioes/{microregionId}/municipios", _jsonOptions);

            if (response == null)
            {
                return new List<Municipality>();
            }

            var municipalities = new List<Municipality>();
            foreach (IbgeMunicipalityDto item in response)
            {
                var municipality = new Municipality(
                    item.Id,
                    item.Nome,
                    microregionId
                );

                municipalities.Add(municipality);
            }

            return municipalities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar municípios do IBGE para a microrregião {MicroregionId}",
                microregionId);
            throw;
        }
    }

    public async Task<List<Districts>> GetDistrictsAsync(int municipalityId)
    {
        try
        {
            List<IbgeDistrictDto>? response =
                await _httpClient.GetFromJsonAsync<List<IbgeDistrictDto>>(
                    $"localidades/municipios/{municipalityId}/distritos", _jsonOptions);

            if (response == null)
            {
                return new List<Districts>();
            }

            var districts = new List<Districts>();
            foreach (IbgeDistrictDto item in response)
            {
                var district = new Districts(
                    item.Id,
                    item.Nome,
                    municipalityId
                );

                districts.Add(district);
            }

            return districts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar distritos do IBGE para o município {MunicipalityId}", municipalityId);
            throw;
        }
    }

    public async Task<List<SubDistricts>> GetSubDistrictsAsync(int districtId)
    {
        try
        {
            List<IbgeSubDistrictDto>? response = 
                await _httpClient.GetFromJsonAsync<List<IbgeSubDistrictDto>>(
                    $"localidades/distritos/{districtId}/subdistritos", _jsonOptions);

            if (response == null)
            {
                return new List<SubDistricts>();
            }

            var subDistricts = new List<SubDistricts>();
            foreach (IbgeSubDistrictDto item in response)
            {
                var subDistrict = new SubDistricts(
                    (int)item.Id, 
                    item.Nome, 
                    districtId); 

                subDistricts.Add(subDistrict);
            }

            return subDistricts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar subdistritos do IBGE para o distrito {DistrictId}", districtId);
            throw;
        }
    }
}
