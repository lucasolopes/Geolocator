using Application.Commands.IbgeSync;
using Application.Factories;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Handlers;

public class SyncStatesCommandHandler : IRequestHandler<SyncStatesCommand>
{
    private readonly IIbgeService _ibgeService;
    private readonly ILogger<SyncStatesCommandHandler> _logger;
    private readonly IStateRepository _stateRepository;

    public SyncStatesCommandHandler(
        IIbgeService ibgeService,
        ILogger<SyncStatesCommandHandler> logger,
        IStateRepository stateRepository)
    {
        _ibgeService = ibgeService;
        _logger = logger;
        _stateRepository = stateRepository;
    }

    public async Task Handle(SyncStatesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando sincronização de estados do IBGE");

        try
        {
            List<State> ibgeStates = await _ibgeService.GetStatesAsync();

            HashSet<long> statesIds = new(await _stateRepository.GetAllIdsAsync());

            var newStates = ibgeStates.Where(s => !statesIds.Contains(s.Id)).ToList();

            if (newStates.Any())
            {
                _logger.LogInformation("Encontrados {Count} novos estados", newStates.Count);
                await _stateRepository.AddRangeAsync(newStates);
            }

            await _stateRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização de estados");
            throw;
        }
    }
}
