using Application.Commands.ElasticsearchSync;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Geolocator.Controllers;

[ApiController]
[Route("api/elasticsearch")]
public class ElasticsearchController : ControllerBase
{
    private readonly ILogger<ElasticsearchController> _logger;
    private readonly IMediator _mediator;

    public ElasticsearchController(IMediator mediator, ILogger<ElasticsearchController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncElasticsearch()
    {
        try
        {
            _logger.LogInformation("Iniciando sincronização manual com o Elasticsearch");

            SyncElasticsearchResult result = await _mediator.Send(new SyncElasticsearchCommand());

            if (result.Success)
            {
                return Ok(new
                {
                    message = "Sincronização com o Elasticsearch concluída com sucesso",
                    details = new
                    {
                        regionsIndexed = result.RegionsIndexed,
                        statesIndexed = result.StatesIndexed,
                        mesoregionsIndexed = result.MesoregionsIndexed,
                        microRegionsIndexed = result.MicroRegionsIndexed,
                        municipalitiesIndexed = result.MunicipalitiesIndexed,
                        districtsIndexed = result.DistrictsIndexed,
                        subDistrictsIndexed = result.SubDistrictsIndexed
                    }
                });
            }

            return StatusCode(500, new
            {
                message = "Falha parcial na sincronização com o Elasticsearch",
                details = new
                {
                    regionsIndexed = result.RegionsIndexed,
                    statesIndexed = result.StatesIndexed,
                    mesoregionsIndexed = result.MesoregionsIndexed,
                    microRegionsIndexed = result.MicroRegionsIndexed,
                    municipalitiesIndexed = result.MunicipalitiesIndexed,
                    districtsIndexed = result.DistrictsIndexed,
                    subDistrictsIndexed = result.SubDistrictsIndexed
                },
                error = result.ErrorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar com o Elasticsearch");
            return StatusCode(500, new { message = "Erro ao sincronizar com o Elasticsearch", error = ex.Message });
        }
    }
}
