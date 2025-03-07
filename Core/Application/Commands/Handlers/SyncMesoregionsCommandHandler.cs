using Application.Commands.IbgeSync;
using Application.Factories;
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
            List<State> states = await _stateRepository.GetAllAsync();
            _logger.LogInformation("Obtidos {Count} estados para sincronização de mesorregiões", states.Count);

            int totalProcessed = 0;

            foreach (State state in states)
            {
                List<Mesoregion> ibgeMesoregions = await _ibgeService.GetMesoregionsAsync(state.Id);
                _logger.LogInformation("Obtidas {Count} mesorregiões da API do IBGE para o estado {StateId}", 
                    ibgeMesoregions.Count, state.Id);

                List<Mesoregion> existingMesoregions = await _mesoregionRepository.GetByStateIdAsync(state.Id);
                
                foreach (Mesoregion ibgeMesoregion in ibgeMesoregions)
                {
                    Mesoregion? existingMesoregion = existingMesoregions.FirstOrDefault(m => m.Id == ibgeMesoregion.Id);

                    if (existingMesoregion == null)
                    {
                        Mesoregion newMesoregion = MesoregionFactory.Create(
                            ibgeMesoregion.Id,
                            ibgeMesoregion.Name,
                            state.Id);
                            
                        await _mesoregionRepository.AddAsync(newMesoregion);
                        _logger.LogInformation("Adicionada nova mesorregião: {Name}", ibgeMesoregion.Name);
                    }
                    else
                    {
                        _logger.LogInformation("Mesorregião já existe: {Name}", existingMesoregion.Name);

                        await _mesoregionRepository.DeleteAsync(existingMesoregion);
                                
                        Mesoregion updatedMesoregion = MesoregionFactory.Create(
                            ibgeMesoregion.Id,
                            ibgeMesoregion.Name,
                            state.Id);
                                    
                        await _mesoregionRepository.AddAsync(updatedMesoregion);
                    }
                    
                    totalProcessed++;
                }
                
                await _mesoregionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Sincronização de mesorregiões concluída com sucesso. Processadas {Count} mesorregiões", totalProcessed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização de mesorregiões");
            throw;
        }
    }
}
