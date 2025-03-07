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
            _logger.LogInformation("Obtidos {Count} estados da API do IBGE", ibgeStates.Count);

            List<State> existingStates = await _stateRepository.GetAllAsync();
            _logger.LogInformation("Existem {Count} estados no banco de dados", existingStates.Count);

            int processedCount = 0;

            foreach (State ibgeState in ibgeStates)
            {
                State? existingState = existingStates.FirstOrDefault(s => s.Id == ibgeState.Id);

                if (existingState == null)
                {
                    State newState = StateFactory.Create(
                        ibgeState.Id,
                        ibgeState.Name,
                        ibgeState.Initials,
                        ibgeState.RegionId);
                        
                    await _stateRepository.AddAsync(newState);
                    _logger.LogInformation("Adicionado novo estado: {Name}", ibgeState.Name);
                }
                else
                {
                    _logger.LogInformation("Estado já existe: {Name}", existingState.Name);

                    await _stateRepository.DeleteAsync(existingState);
                            
                    State updatedState = StateFactory.Create(
                        ibgeState.Id,
                        ibgeState.Name,
                        ibgeState.Initials,
                        ibgeState.RegionId);
                                
                    await _stateRepository.AddAsync(updatedState);
                }
                
                processedCount++;
            }
            
            await _stateRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sincronização de estados concluída com sucesso. Processados {Count} estados", processedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização de estados");
            throw;
        }
    }
}
