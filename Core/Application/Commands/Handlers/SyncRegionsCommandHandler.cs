using Application.Commands.IbgeSync;
using Application.DTOs;
using Application.Factories;
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
            List<IbgeRegionDto> ibgeRegions = await _ibgeService.GetRegionsFromIbgeAsync();
            _logger.LogInformation("Obtidas {Count} regiões da API do IBGE", ibgeRegions.Count);

            List<Region> existingRegions = await _regionRepository.GetAllAsync();
            _logger.LogInformation("Existem {Count} regiões no banco de dados", existingRegions.Count);

            int processedCount = 0;

            foreach (IbgeRegionDto ibgeRegion in ibgeRegions)
            {
                Region? existingRegion = existingRegions.FirstOrDefault(r => r.Id == ibgeRegion.Id);

                var regions = new List<Region>();

                if (existingRegion == null)
                {
                    Region newRegion = RegionFactory.Create(
                        ibgeRegion.Id,
                        ibgeRegion.Nome,
                        ibgeRegion.Sigla);
                        
                    await _regionRepository.AddAsync(newRegion);
                    regions.Add(newRegion);
                    _logger.LogInformation("Adicionada nova região: {Name}", ibgeRegion.Nome);
                }
                else
                {
                      _logger.LogInformation("Região já existe: {Name}", existingRegion.Name);

                      await _regionRepository.DeleteAsync(existingRegion);
                            
                      Region updatedRegion = RegionFactory.Create(
                          ibgeRegion.Id,
                          ibgeRegion.Nome,
                          ibgeRegion.Sigla);
                                
                      await _regionRepository.AddAsync(updatedRegion);
                      regions.Add(updatedRegion);
                }
                processedCount++;
            }
            await _regionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sincronização de regiões concluída com sucesso. Processadas {Count} regiões", processedCount);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Erro durante a sincronização de regiões. Processadas 0 regiões");
           throw;
        }
    }
}
