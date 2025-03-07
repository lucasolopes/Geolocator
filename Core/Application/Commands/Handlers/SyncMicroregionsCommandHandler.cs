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
            List<Mesoregion> mesoregions = await _mesoregionRepository.GetAllAsync();
            _logger.LogInformation("Obtidas {Count} mesorregiões para sincronização de microrregiões", mesoregions.Count);

            int totalProcessed = 0;

            foreach (Mesoregion mesoregion in mesoregions)
            {
                List<MicroRegion> ibgeMicroregions = await _ibgeService.GetMicroregionsAsync(mesoregion.Id);
                _logger.LogInformation("Obtidas {Count} microrregiões da API do IBGE para a mesorregião {MesoregionId}", 
                    ibgeMicroregions.Count, mesoregion.Id);

                List<MicroRegion> existingMicroregions = await _microRegionRepository.GetByMesoregionIdAsync(mesoregion.Id);
                
                foreach (MicroRegion ibgeMicroregion in ibgeMicroregions)
                {
                    MicroRegion? existingMicroregion = existingMicroregions.FirstOrDefault(m => m.Id == ibgeMicroregion.Id);

                    if (existingMicroregion == null)
                    {
                        MicroRegion newMicroregion = MicroRegionFactory.Create(
                            ibgeMicroregion.Id,
                            ibgeMicroregion.Name,
                            mesoregion.Id);
                            
                        await _microRegionRepository.AddAsync(newMicroregion);
                        _logger.LogInformation("Adicionada nova microrregião: {Name}", ibgeMicroregion.Name);
                    }
                    else
                    {
                        _logger.LogInformation("Microrregião já existe: {Name}", existingMicroregion.Name);

                        await _microRegionRepository.DeleteAsync(existingMicroregion);
                                
                        MicroRegion updatedMicroregion = MicroRegionFactory.Create(
                            ibgeMicroregion.Id,
                            ibgeMicroregion.Name,
                            mesoregion.Id);
                                    
                        await _microRegionRepository.AddAsync(updatedMicroregion);
                    }
                    
                    totalProcessed++;
                }
                
                await _microRegionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Sincronização de microrregiões concluída com sucesso. Processadas {Count} microrregiões", totalProcessed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização de microrregiões");
            throw;
        }
    }
}
