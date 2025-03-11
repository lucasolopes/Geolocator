using Application.Commands.IbgeSync;
using Application.Factories;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Handlers;

public class SyncMicroregionsCommandHandler : IRequestHandler<SyncMicroregionsCommand>
{
    private readonly IIbgeService _ibgeService;
    private readonly ILogger<SyncMicroregionsCommandHandler> _logger;
    private readonly IMicroRegionRepository _microRegionRepository;
    private readonly IMesoregionRepository _mesoregionRepository;

    public SyncMicroregionsCommandHandler(
        IIbgeService ibgeService,
        ILogger<SyncMicroregionsCommandHandler> logger,
        IMicroRegionRepository microRegionRepository,
        IMesoregionRepository mesoregionRepository)
    {
        _ibgeService = ibgeService;
        _logger = logger;
        _microRegionRepository = microRegionRepository;
        _mesoregionRepository = mesoregionRepository;
    }

    public async Task Handle(SyncMicroregionsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando sincronização de microrregiões do IBGE");

        try
        {
            List<MicroRegion> ibgeMicroregions = await _ibgeService.GetMicroregionsAsync();

            HashSet<long> microregionsIds = new(await _microRegionRepository.GetAllIdsAsync());

            var newMicroregions = ibgeMicroregions.Where(m => !microregionsIds.Contains(m.Id)).ToList();

            if (newMicroregions.Any())
            {
                _logger.LogInformation("Encontradas {Count} novas microrregiões", newMicroregions.Count);
                await _microRegionRepository.AddRangeAsync(newMicroregions);
            }

            await _microRegionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sincronização de microrregiões concluída com sucesso. Processadas {Count} microrregiões", newMicroregions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização de microrregiões");
            throw;
        }
    }
}
