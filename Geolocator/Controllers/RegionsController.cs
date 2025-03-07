using Application.DTOs;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Geolocator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegionsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Obtém todas as regiões
    /// </summary>
    /// <returns>Lista de regiões</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<RegionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RegionDto>>> GetAll()
    {
        var query = new GetAllRegionsQuery();
        List<RegionDto> result = await _mediator.Send(query);
        return Ok(result);
    }
}
