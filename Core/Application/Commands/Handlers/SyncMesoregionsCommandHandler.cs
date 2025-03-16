using Application.Commands.IbgeSync;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Handlers;

public class SyncMesoregionsCommandHandler : IRequestHandler<SyncMesoregionsCommand>
{
    private readonly IIbgeService _ibgeService;
    private readonly ILogger<SyncMesoregionsCommandHandler> _logger;
    private readonly IMesoregionRepository _mesoregionRepository;
    private readonly IStateRepository _stateRepository;

    public SyncMesoregionsCommandHandler(
        IIbgeService ibgeService,
        ILogger<SyncMesoregionsCommandHandler> logger,
        IMesoregionRepository mesoregionRepository,
        IStateRepository stateRepository)
    {
        _ibgeService = ibgeService;
        _logger = logger;
        _mesoregionRepository = mesoregionRepository;
        _stateRepository = stateRepository;
    }

    public async Task Handle(SyncMesoregionsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando sincronização de mesorregiões do IBGE");

        try
        {
            List<Mesoregion> ibgeMesoregions = await _ibgeService.GetMesoregionsAsync();

            HashSet<long> mesoregionsIds = new(await _mesoregionRepository.GetAllIdsAsync());

            var newMesoregions = ibgeMesoregions.Where(m => !mesoregionsIds.Contains(m.Id)).ToList();

            if (newMesoregions.Any())
            {
                _logger.LogInformation("Encontradas {Count} novas mesorregiões", newMesoregions.Count);
                await _mesoregionRepository.AddRangeAsync(newMesoregions);
            }

            await _mesoregionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Sincronização de mesorregiões concluída com sucesso. Processadas {Count} mesorregiões",
                newMesoregions.Count);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro durante a sincronização de mesorregiões");
            throw;
        }
    }
}
