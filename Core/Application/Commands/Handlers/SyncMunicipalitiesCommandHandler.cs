using Application.Commands.IbgeSync;
using Application.Factories;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Handlers;

public class SyncMunicipalitiesCommandHandler : IRequestHandler<SyncMunicipalitiesCommand>
{
    private readonly IIbgeService _ibgeService;
    private readonly ILogger<SyncMunicipalitiesCommandHandler> _logger;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IMicroRegionRepository _microRegionRepository;

    public SyncMunicipalitiesCommandHandler(
        IIbgeService ibgeService,
        ILogger<SyncMunicipalitiesCommandHandler> logger,
        IMunicipalityRepository municipalityRepository,
        IMicroRegionRepository microRegionRepository)
    {
        _ibgeService = ibgeService;
        _logger = logger;
        _municipalityRepository = municipalityRepository;
        _microRegionRepository = microRegionRepository;
    }

    public async Task Handle(SyncMunicipalitiesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando sincronização de municípios do IBGE");

        try
        {
            List<MicroRegion> microRegions = await _microRegionRepository.GetAllAsync();
            _logger.LogInformation("Obtidas {Count} microrregiões para sincronização de municípios", microRegions.Count);

            int totalProcessed = 0;

            foreach (MicroRegion microRegion in microRegions)
            {
                List<Municipality> ibgeMunicipalities = await _ibgeService.GetMunicipalitiesAsync(microRegion.Id);
                _logger.LogInformation("Obtidos {Count} municípios da API do IBGE para a microrregião {MicroRegionId}", 
                    ibgeMunicipalities.Count, microRegion.Id);

                List<Municipality> existingMunicipalities = await _municipalityRepository.GetByMicroRegionIdAsync(microRegion.Id);
                
                foreach (Municipality ibgeMunicipality in ibgeMunicipalities)
                {
                    Municipality? existingMunicipality = existingMunicipalities.FirstOrDefault(m => m.Id == ibgeMunicipality.Id);

                    if (existingMunicipality == null)
                    {
                        Municipality newMunicipality = MunicipalityFactory.Create(
                            ibgeMunicipality.Id,
                            ibgeMunicipality.Name,
                            microRegion.Id);
                            
                        await _municipalityRepository.AddAsync(newMunicipality);
                        _logger.LogInformation("Adicionado novo município: {Name}", ibgeMunicipality.Name);
                    }
                    else
                    {
                        _logger.LogInformation("Município já existe: {Name}", existingMunicipality.Name);

                        await _municipalityRepository.DeleteAsync(existingMunicipality);
                                
                        Municipality updatedMunicipality = MunicipalityFactory.Create(
                            ibgeMunicipality.Id,
                            ibgeMunicipality.Name,
                            microRegion.Id);
                                    
                        await _municipalityRepository.AddAsync(updatedMunicipality);
                    }
                    
                    totalProcessed++;
                }
                
                await _municipalityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Sincronização de municípios concluída com sucesso. Processados {Count} municípios", totalProcessed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização de municípios");
            throw;
        }
    }
}
