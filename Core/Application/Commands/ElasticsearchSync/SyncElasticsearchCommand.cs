using MediatR;

namespace Application.Commands.ElasticsearchSync;

public record SyncElasticsearchCommand() : IRequest<SyncElasticsearchResult>;

public record SyncElasticsearchResult(
    bool Success, 
    bool RegionsIndexed,
    bool StatesIndexed,
    bool MesoregionsIndexed,
    bool MicroRegionsIndexed,
    bool MunicipalitiesIndexed,
    bool DistrictsIndexed,
    bool SubDistrictsIndexed,
    string? ErrorMessage = null);
