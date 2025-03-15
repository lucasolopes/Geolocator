using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ExternalServices.HttpClients;

public class IbgeService : IIbgeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IbgeService> _logger;

    public IbgeService(HttpClient httpClient, ILogger<IbgeService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _httpClient.BaseAddress = new Uri("https://servicodados.ibge.gov.br/api/v1/");
    }

    public async Task<List<Region>> GetRegionsAsync()
    {
        try
        {
            List<dynamic>? json = await GetJsonAsync("localidades/regioes");

            return json.Select(e => new Region
            {
                Id = int.Parse(e["id"].ToString()),
                Name = e["nome"].ToString(),
                Initials = e["sigla"].ToString()
            }).ToList();
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
            List<dynamic>? json = await GetJsonAsync("localidades/estados");

            return json.Select(e => new State
            {
                Id = int.Parse(e["id"].ToString()),
                Name = e["nome"].ToString(),
                Initials = e["sigla"].ToString(),
                RegionId = int.Parse(e["regiao"]["id"].ToString())
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estados do IBGE");
            throw;
        }
    }

    public async Task<List<Mesoregion>> GetMesoregionsAsync()
    {
        try
        {
            List<dynamic>? json = await GetJsonAsync("localidades/mesorregioes");

            return json.Select(e => new Mesoregion
            {
                Id = int.Parse(e["id"].ToString()),
                Name = e["nome"].ToString(),
                StateId = int.Parse(e["UF"]["id"].ToString())
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar mesorregiões do IBGE");
            throw;
        }
    }

    public async Task<List<MicroRegion>> GetMicroregionsAsync()
    {
        try
        {
            List<dynamic>? json = await GetJsonAsync("localidades/microrregioes");

            return json.Select(e => new MicroRegion
            {
                Id = int.Parse(e["id"].ToString()),
                Name = e["nome"].ToString(),
                MesoregionId = int.Parse(e["mesorregiao"]["id"].ToString())
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar microrregiões do IBGE");
            throw;
        }
    }

    public async Task<List<Municipality>> GetMunicipalitiesAsync()
    {
        try
        {
            List<dynamic>? json = await GetJsonAsync("localidades/municipios");

            return json.Select(e => new Municipality
            {
                Id = int.Parse(e["id"].ToString()),
                Name = e["nome"].ToString(),
                MicroRegionId = int.Parse(e["microrregiao"]["id"].ToString())
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar municípios do IBGE");
            throw;
        }
    }

    public async Task<List<Districts>> GetDistrictsAsync()
    {
        try
        {
            List<dynamic>? json = await GetJsonAsync("localidades/distritos");

            return json.Select(e => new Districts
            {
                Id = int.Parse(e["id"].ToString()),
                Name = e["nome"].ToString(),
                MunicipalityId = int.Parse(e["municipio"]["id"].ToString())
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar distritos do IBGE");
            throw;
        }
    }

    public async Task<List<SubDistricts>> GetSubDistrictsAsync()
    {
        try
        {
            List<dynamic>? json = await GetJsonAsync("localidades/subdistritos");

            return json.Select(e => new SubDistricts
            {
                Id = long.Parse(e["id"].ToString()),
                Name = e["nome"].ToString(),
                DistrictId = int.Parse(e["distrito"]["id"].ToString())
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar subdistritos do IBGE para o distrito");
            throw;
        }
    }

    private async Task<List<dynamic>?> GetJsonAsync(string url)
    {
        try
        {
            string response = await _httpClient.GetStringAsync(url);
            if (response == null)
            {
                return new List<dynamic>();
            }

            return JsonConvert.DeserializeObject<List<dynamic>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar dados do IBGE");
            throw;
        }
    }
}
