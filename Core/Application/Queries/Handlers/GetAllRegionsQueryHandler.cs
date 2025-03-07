using Application.DTOs;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Handlers;

public class GetAllRegionsQueryHandler : IRequestHandler<GetAllRegionsQuery, List<RegionDto>>
{
    private readonly IRegionRepository _regionRepository;

    public GetAllRegionsQueryHandler(IRegionRepository regionRepository)
    {
        _regionRepository = regionRepository;
    }

    public async Task<List<RegionDto>> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
    {
        List<Region> regions = await _regionRepository.GetAllAsync();
        
        return regions.Select(r => new RegionDto
        {
            Id = r.Id,
            Name = r.Name,
            Initials = r.Initials
        }).ToList();
    }
}
