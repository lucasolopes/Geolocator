using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs;
using Application.Interfaces.Services;
using Domain.Entities;

namespace ExternalServices.HttpClients;

public class IbgeService : IIbgeService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    //private readonly ILogger<IbgeService> _logger;

    public IbgeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
      //  _logger = logger;

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
            throw;
        }
    }

    public async Task<List<State>> GetStatesAsync()
    {/*
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
                // Implementação similar à de regiões
                var state = new State
                {
                    // Id = item.Id,
                    // Name = item.Nome,
                    // Initials = item.Sigla,
                    // RegionId = item.Regiao.Id
                };

                states.Add(state);
            }

            return states;
        }
        catch (Exception ex)
        {
          //  _logger.LogError(ex, "Erro ao buscar estados do IBGE");
            throw;
        }*/throw new NotImplementedException();
    }

    public async Task<List<Mesoregion>> GetMesoregionsAsync(int stateId)
    {/*
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
                var mesoregion = new Mesoregion
                {
                    // Id = item.Id,
                    // Name = item.Nome,
                    // StateId = stateId
                };

                mesoregions.Add(mesoregion);
            }

            return mesoregions;
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Erro ao buscar mesorregiões do IBGE para o estado {StateId}", stateId);
            throw;
        }*/throw new NotImplementedException();
    }

    public async Task<List<MicroRegion>> GetMicroregionsAsync(int mesoregionId)
    {
        throw new NotImplementedException();
        /*
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
                var microregion = new MicroRegion
                {
                    // Id = item.Id,
                    // Name = item.Nome,
                    // MesoregionId = mesoregionId
                };

                microregions.Add(microregion);
            }

            return microregions;
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Erro ao buscar microrregiões do IBGE para a mesorregião {MesoregionId}",mesoregionId);
            throw;
        }*/
    }

    public async Task<List<Municipality>> GetMunicipalitiesAsync(int microregionId)
    {/*
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
                var municipality = new Municipality
                {
                    // Id = item.Id,
                    // Name = item.Nome,
                    // MicroRegionId = microregionId
                };

                municipalities.Add(municipality);
            }

            return municipalities;
        }
        catch (Exception ex)
        {
           // _logger.LogError(ex, "Erro ao buscar municípios do IBGE para a microrregião {MicroregionId}",microregionId);
            throw;
        }*/
        throw new NotImplementedException();
    }

    public async Task<List<Districts>> GetDistrictsAsync(int municipalityId)
    {/*
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
                var district = new Districts
                {
                    // Id = item.Id,
                    // Name = item.Nome,
                    // MunicipalityId = municipalityId
                };

                districts.Add(district);
            }

            return districts;
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Erro ao buscar distritos do IBGE para o município {MunicipalityId}", municipalityId);
            throw;
        }*/
        throw new NotImplementedException();
    }

    public async Task<List<SubDistricts>> GetSubDistrictsAsync(int districtId)
    {/*
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
                var subDistrict = new SubDistricts
                {
                    // Id = item.Id,
                    // Name = item.Nome,
                    // DistrictId = districtId.ToString()
                };

                subDistricts.Add(subDistrict);
            }

            return subDistricts;
        }
        catch (Exception ex)
        {
           // _logger.LogError(ex, "Erro ao buscar subdistritos do IBGE para o distrito {DistrictId}", districtId);
            throw;
        }*/
        throw new NotImplementedException();
    }
}


/*
public class IbgeStateDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Sigla { get; set; }
    public IbgeRegionDto Regiao { get; set; }
}

public class IbgeMesoregionDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public IbgeStateDto UF { get; set; }
}

public class IbgeMicroregionDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public IbgeMesoregionDto Mesorregiao { get; set; }
}

public class IbgeMunicipalityDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public IbgeMicroregionDto Microrregiao { get; set; }
}

public class IbgeDistrictDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public IbgeMunicipalityDto Municipio { get; set; }
}

public class IbgeSubDistrictDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public IbgeDistrictDto Distrito { get; set; }
}
*/
