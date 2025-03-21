﻿using Application.Commands.ElasticsearchSync;
using Application.Commands.IbgeSync;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Geolocator.Controllers;

[ApiController]
[Route("api/ibge")]
public class IbgeSyncController : ControllerBase
{
    private readonly ILogger<IbgeSyncController> _logger;
    private readonly IMediator _mediator;

    public IbgeSyncController(IMediator mediator, ILogger<IbgeSyncController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncIbgeData()
    {
        try
        {
            _logger.LogInformation("Iniciando sincronização manual dos dados do IBGE");

            await _mediator.Send(new SyncRegionsCommand());

            await _mediator.Send(new SyncStatesCommand());

            await _mediator.Send(new SyncMesoregionsCommand());

            await _mediator.Send(new SyncMicroregionsCommand());

            await _mediator.Send(new SyncMunicipalitiesCommand());

            await _mediator.Send(new SyncDistrictsCommand());

            await _mediator.Send(new SyncSubDistrictsCommand());

            _logger.LogInformation(
                "Sincronização manual do IBGE concluída com sucesso. Sincronizando com Elasticsearch...");

            SyncElasticsearchResult elasticsearchResult = await _mediator.Send(new SyncElasticsearchCommand());

            return Ok(new
            {
                message = "Sincronização dos dados do IBGE e Elasticsearch concluída com sucesso",
                elasticsearchSync = new
                {
                    success = elasticsearchResult.Success,
                    details = new
                    {
                        regionsIndexed = elasticsearchResult.RegionsIndexed,
                        statesIndexed = elasticsearchResult.StatesIndexed,
                        mesoregionsIndexed = elasticsearchResult.MesoregionsIndexed,
                        microRegionsIndexed = elasticsearchResult.MicroRegionsIndexed,
                        municipalitiesIndexed = elasticsearchResult.MunicipalitiesIndexed,
                        districtsIndexed = elasticsearchResult.DistrictsIndexed,
                        subDistrictsIndexed = elasticsearchResult.SubDistrictsIndexed
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar dados do IBGE manualmente");
            return StatusCode(500, new { message = "Erro ao sincronizar dados do IBGE", error = ex.Message });
        }
    }

    [HttpGet("regions")]
    public async Task<IActionResult> SyncRegions()
    {
        try
        {
            await _mediator.Send(new SyncRegionsCommand());
            return Ok(new { message = "Sincronização de regiões concluída com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar regiões do IBGE");
            return StatusCode(500, new { message = "Erro ao sincronizar regiões do IBGE", error = ex.Message });
        }
    }

    [HttpGet("states")]
    public async Task<IActionResult> SyncStates()
    {
        try
        {
            await _mediator.Send(new SyncStatesCommand());
            return Ok(new { message = "Sincronização de estados concluída com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar estados do IBGE");
            return StatusCode(500, new { message = "Erro ao sincronizar estados do IBGE", error = ex.Message });
        }
    }

    [HttpGet("mesoregions")]
    public async Task<IActionResult> SyncMesoregions()
    {
        try
        {
            await _mediator.Send(new SyncMesoregionsCommand());
            return Ok(new { message = "Sincronização de mesorregiões concluída com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar mesorregiões do IBGE");
            return StatusCode(500, new { message = "Erro ao sincronizar mesorregiões do IBGE", error = ex.Message });
        }
    }

    [HttpGet("microregions")]
    public async Task<IActionResult> SyncMicroregions()
    {
        try
        {
            await _mediator.Send(new SyncMicroregionsCommand());
            return Ok(new { message = "Sincronização de microrregiões concluída com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar microrregiões do IBGE");
            return StatusCode(500, new { message = "Erro ao sincronizar microrregiões do IBGE", error = ex.Message });
        }
    }

    [HttpGet("municipalities")]
    public async Task<IActionResult> SyncMunicipalities()
    {
        try
        {
            await _mediator.Send(new SyncMunicipalitiesCommand());
            return Ok(new { message = "Sincronização de municípios concluída com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar municípios do IBGE");
            return StatusCode(500, new { message = "Erro ao sincronizar municípios do IBGE", error = ex.Message });
        }
    }

    [HttpGet("districts")]
    public async Task<IActionResult> SyncDistricts()
    {
        try
        {
            await _mediator.Send(new SyncDistrictsCommand());
            return Ok(new { message = "Sincronização de distritos concluída com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar distritos do IBGE");
            return StatusCode(500, new { message = "Erro ao sincronizar distritos do IBGE", error = ex.Message });
        }
    }

    [HttpGet("subdistricts")]
    public async Task<IActionResult> SyncSubDistricts()
    {
        try
        {
            await _mediator.Send(new SyncSubDistrictsCommand());
            return Ok(new { message = "Sincronização de subdistritos concluída com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar subdistritos do IBGE");
            return StatusCode(500, new { message = "Erro ao sincronizar subdistritos do IBGE", error = ex.Message });
        }
    }
}
