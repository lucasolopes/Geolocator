using Application.Commands.IbgeSync;
using Application.Factories;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Handlers;

public class SyncDistrictsCommandHandler : IRequestHandler<SyncDistrictsCommand>
{
    private readonly IIbgeService _ibgeService;
    private readonly ILogger<SyncDistrictsCommandHandler> _logger;
    private readonly IDistrictsRepository _districtsRepository;
    private readonly IMunicipalityRepository _municipalityRepository;

    public SyncDistrictsCommandHandler(
        IIbgeService ibgeService,
        ILogger<SyncDistrictsCommandHandler> logger,
        IDistrictsRepository districtsRepository,
        IMunicipalityRepository municipalityRepository)
    {
        _ibgeService = ibgeService;
        _logger = logger;
        _districtsRepository = districtsRepository;
        _municipalityRepository = municipalityRepository;
    }

    public async Task Handle(SyncDistrictsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando sincronização de distritos do IBGE");

        try
        {
            List<Municipality> municipalities = await _municipalityRepository.GetAllAsync();
            _logger.LogInformation("Obtidos {Count} municípios para sincronização de distritos", municipalities.Count);

            int totalProcessed = 0;

            foreach (Municipality municipality in municipalities)
            {
                List<Districts> ibgeDistricts = await _ibgeService.GetDistrictsAsync(municipality.Id);
                _logger.LogInformation("Obtidos {Count} distritos da API do IBGE para o município {MunicipalityId}", 
                    ibgeDistricts.Count, municipality.Id);

                List<Districts> existingDistricts = await _districtsRepository.GetByMunicipalityIdAsync(municipality.Id);
                
                foreach (Districts ibgeDistrict in ibgeDistricts)
                {
                    Districts? existingDistrict = existingDistricts.FirstOrDefault(d => d.Id == ibgeDistrict.Id);

                    if (existingDistrict == null)
                    {
                        Districts newDistrict = DistrictsFactory.Create(
                            ibgeDistrict.Id,
                            ibgeDistrict.Name,
                            municipality.Id);
                            
                        await _districtsRepository.AddAsync(newDistrict);
                        _logger.LogInformation("Adicionado novo distrito: {Name}", ibgeDistrict.Name);
                    }
                    else
                    {
                        _logger.LogInformation("Distrito já existe: {Name}", existingDistrict.Name);

                        await _districtsRepository.DeleteAsync(existingDistrict);
                                
                        Districts updatedDistrict = DistrictsFactory.Create(
                            ibgeDistrict.Id,
                            ibgeDistrict.Name,
                            municipality.Id);
                                    
                        await _districtsRepository.AddAsync(updatedDistrict);
                    }
                    
                    totalProcessed++;
                }
                
                await _districtsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Sincronização de distritos concluída com sucesso. Processados {Count} distritos", totalProcessed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização de distritos");
            throw;
        }
    }
}
