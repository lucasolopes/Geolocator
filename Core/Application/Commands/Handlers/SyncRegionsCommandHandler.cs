using Application.Commands.IbgeSync;
using Application.DTOs;
using Application.Factories;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Handlers;

public class SyncRegionsCommandHandler(
    IIbgeService ibgeService,
  //  ILogger<SyncRegionsCommandHandler> logger,
    IRegionRepository repository) : IRequestHandler<SyncRegionsCommand>
{
    private readonly IIbgeService _ibgeService = ibgeService;
  //  private readonly ILogger<SyncRegionsCommandHandler> _logger = logger;
    private readonly IRegionRepository _regionRepository = repository;

    public async Task Handle(SyncRegionsCommand request, CancellationToken cancellationToken)
    {
       // _logger.LogInformation("Iniciando sincronização de regiões do IBGE");

        try
        {
            // Busca regiões da API do IBGE
            List<IbgeRegionDto> ibgeRegions = await _ibgeService.GetRegionsFromIbgeAsync();
          //  _logger.LogInformation("Obtidas {Count} regiões da API do IBGE", ibgeRegions.Count);

            // Busca regiões existentes no banco de dados
            List<Region> existingRegions = await _regionRepository.GetAllAsync();
          //  _logger.LogInformation("Existem {Count} regiões no banco de dados", existingRegions.Count);


            // Para cada região da API
            foreach (IbgeRegionDto ibgeRegion in ibgeRegions)
            {
                // Verifica se a região já existe no banco
                Region? existingRegion = existingRegions.FirstOrDefault(r => r.Id == ibgeRegion.Id);

                var regions = new List<Region>();

                if (existingRegion == null)
                {
                    // Se não existir, adiciona
                    Region newRegion = RegionFactory.Create(
                        ibgeRegion.Id,
                        ibgeRegion.Nome,
                        ibgeRegion.Sigla);
                        
                    await _regionRepository.AddAsync(newRegion);
                    regions.Add(newRegion);
              //      _logger.LogInformation("Adicionada nova região: {Name}", ibgeRegion.Name);
                }
                else
                {
                    // Se existir e tiver alterações, atualiza
                    // Aqui precisamos implementar um método Update na entidade ou usar um projeto como o AutoMapper
                    // Por simplicidade, vamos apenas registrar que encontramos a região
              //      _logger.LogInformation("Região já existe: {Name}", existingRegion.Name);

                      await _regionRepository.DeleteAsync(existingRegion);
                            
                      Region updatedRegion = RegionFactory.Create(
                          ibgeRegion.Id,
                          ibgeRegion.Nome,
                          ibgeRegion.Sigla);
                                
                      await _regionRepository.AddAsync(updatedRegion);
                      regions.Add(updatedRegion);
                }

            }

            // Salva todas as mudanças
            await _regionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);


         //   _logger.LogInformation("Sincronização de regiões concluída com sucesso. Processadas {Count} regiões", processedCount);

        }
        catch (Exception ex)
        {

           // _logger.LogInformation("Erro durante a sincronização de regiões. Processadas 0 regiões");
           throw;

        }
    }
}

