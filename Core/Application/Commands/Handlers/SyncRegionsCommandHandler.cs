using Application.Commands.IbgeSync;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Handlers;

public class SyncRegionsCommandHandler(
    IIbgeService ibgeService,
    ILogger<SyncRegionsCommandHandler> logger,
    IRegionRepository repository) : IRequestHandler<SyncRegionsCommand>
{
    private readonly IIbgeService _ibgeService = ibgeService;
    private readonly ILogger<SyncRegionsCommandHandler> _logger = logger;
    private readonly IRegionRepository _regionRepository = repository;

    public async Task Handle(SyncRegionsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando sincronização de regiões do IBGE");

        try
        {
            List<Region> ibgeRegions = await _ibgeService.GetRegionsAsync();

            HashSet<long> regionsIds = new(await _regionRepository.GetAllIdsAsync());

            var newRegions = ibgeRegions.Where(r => !regionsIds.Contains(r.Id)).ToList();

            if (newRegions.Any())
            {
                _logger.LogInformation("Encontradas {Count} novas regiões", newRegions.Count);
                await _regionRepository.AddRangeAsync(newRegions);
            }

            await _regionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sincronização de regiões concluída com sucesso. Processadas {Count} regiões",
                newRegions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Erro durante a sincronização de regiões. Processadas 0 regiões");
            throw;
        }
    }
}
