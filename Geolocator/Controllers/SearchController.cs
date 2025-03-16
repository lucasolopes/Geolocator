using Application.Queries.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Geolocator.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ILogger<SearchController> _logger;
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator, ILogger<SearchController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeRegions = true,
        [FromQuery] bool includeStates = true,
        [FromQuery] bool includeMesoregions = true,
        [FromQuery] bool includeMicroRegions = true,
        [FromQuery] bool includeMunicipalities = true,
        [FromQuery] bool includeDistricts = true,
        [FromQuery] bool includeSubDistricts = true)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "O termo de busca é obrigatório" });
        }

        if (q.Length < 2)
        {
            return BadRequest(new { message = "O termo de busca deve ter pelo menos 2 caracteres" });
        }

        try
        {
            _logger.LogInformation("Iniciando busca pelo termo '{Query}'", q);

            var query = new SearchLocationsQuery(
                q,
                page,
                pageSize,
                includeRegions,
                includeStates,
                includeMesoregions,
                includeMicroRegions,
                includeMunicipalities,
                includeDistricts,
                includeSubDistricts);

            SearchLocationsResult result = await _mediator.Send(query);

            return Ok(new
            {
                query = q,
                page,
                pageSize,
                results = new
                {
                    regions = result.Regions,
                    states = result.States,
                    mesoregions = result.Mesoregions,
                    microRegions = result.MicroRegions,
                    municipalities = result.Municipalities,
                    districts = result.Districts,
                    subDistricts = result.SubDistricts
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar busca pelo termo '{Query}'", q);
            return StatusCode(500, new { message = "Ocorreu um erro ao processar sua busca", error = ex.Message });
        }
    }

    [HttpGet("regions")]
    public async Task<IActionResult> SearchRegions([FromQuery] string q, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "O termo de busca é obrigatório" });
        }

        try
        {
            var query = new SearchLocationsQuery(
                q, page, pageSize,
                true,
                false,
                false,
                false,
                false,
                false,
                false);

            SearchLocationsResult result = await _mediator.Send(query);

            return Ok(new
            {
                query = q,
                page,
                pageSize,
                results = result.Regions
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar regiões pelo termo '{Query}'", q);
            return StatusCode(500, new { message = "Ocorreu um erro ao buscar regiões", error = ex.Message });
        }
    }

    [HttpGet("states")]
    public async Task<IActionResult> SearchStates([FromQuery] string q, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "O termo de busca é obrigatório" });
        }

        try
        {
            var query = new SearchLocationsQuery(
                q, page, pageSize,
                false,
                true,
                false,
                false,
                false,
                false,
                false);

            SearchLocationsResult result = await _mediator.Send(query);

            return Ok(new
            {
                query = q,
                page,
                pageSize,
                results = result.States
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estados pelo termo '{Query}'", q);
            return StatusCode(500, new { message = "Ocorreu um erro ao buscar estados", error = ex.Message });
        }
    }

    [HttpGet("municipalities")]
    public async Task<IActionResult> SearchMunicipalities([FromQuery] string q, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "O termo de busca é obrigatório" });
        }

        try
        {
            var query = new SearchLocationsQuery(
                q, page, pageSize,
                false,
                false,
                false,
                false,
                true,
                false,
                false);

            SearchLocationsResult result = await _mediator.Send(query);

            return Ok(new
            {
                query = q,
                page,
                pageSize,
                results = result.Municipalities
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar municípios pelo termo '{Query}'", q);
            return StatusCode(500, new { message = "Ocorreu um erro ao buscar municípios", error = ex.Message });
        }
    }
}
