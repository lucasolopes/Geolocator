using Application.DTOs;
using MediatR;

namespace Application.Queries;

public record GetAllRegionsQuery() : IRequest<List<RegionDto>>;
