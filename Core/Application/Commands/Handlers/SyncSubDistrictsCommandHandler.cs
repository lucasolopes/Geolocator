using Application.Commands.IbgeSync;
using Application.Factories;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Handlers;

public class SyncSubDistrictsCommandHandler : IRequestHandler<SyncSubDistrictsCommand>
{
    private readonly IIbgeService _ibgeService;
    private readonly ILogger<SyncSubDistrictsCommandHandler> _logger;
    private readonly ISubDistrictsRepository _subDistrictsRepository;
    private readonly IDistrictsRepository _districtsRepository;

    public SyncSubDistrictsCommandHandler(
        IIbgeService ibgeService,
        ILogger<SyncSubDistrictsCommandHandler> logger,
        ISubDistrictsRepository subDistrictsRepository,
        IDistrictsRepository districtsRepository)
    {
        _ibgeService = ibgeService;
        _logger = logger;
        _subDistrictsRepository = subDistrictsRepository;
        _districtsRepository = districtsRepository;
    }

    public async Task Handle(SyncSubDistrictsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando sincronização de subdistritos do IBGE");

        try
        {
            List<Districts> districts = await _districtsRepository.GetAllAsync();
            _logger.LogInformation("Obtidos {Count} distritos para sincronização de subdistritos", districts.Count);

            int totalProcessed = 0;

            foreach (Districts district in districts)
            {
                List<SubDistricts> ibgeSubDistricts = await _ibgeService.GetSubDistrictsAsync(district.Id);
                _logger.LogInformation("Obtidos {Count} subdistritos da API do IBGE para o distrito {DistrictId}", 
                    ibgeSubDistricts.Count, district.Id);

                List<SubDistricts> existingSubDistricts = await _subDistrictsRepository.GetByDistrictIdAsync(district.Id);
                
                foreach (SubDistricts ibgeSubDistrict in ibgeSubDistricts)
                {
                    SubDistricts? existingSubDistrict = existingSubDistricts.FirstOrDefault(s => s.Id == ibgeSubDistrict.Id);

                    if (existingSubDistrict == null)
                    {
                        SubDistricts newSubDistrict = SubDistrictsFactory.Create(
                            ibgeSubDistrict.Id,
                            ibgeSubDistrict.Name,
                            district.Id);
                            
                        await _subDistrictsRepository.AddAsync(newSubDistrict);
                        _logger.LogInformation("Adicionado novo subdistrito: {Name}", ibgeSubDistrict.Name);
                    }
                    else
                    {
                        _logger.LogInformation("Subdistrito já existe: {Name}", existingSubDistrict.Name);

                        await _subDistrictsRepository.DeleteAsync(existingSubDistrict);
                                
                        SubDistricts updatedSubDistrict = SubDistrictsFactory.Create(
                            ibgeSubDistrict.Id,
                            ibgeSubDistrict.Name,
                            district.Id);
                                    
                        await _subDistrictsRepository.AddAsync(updatedSubDistrict);
                    }
                    
                    totalProcessed++;
                }
                
                await _subDistrictsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Sincronização de subdistritos concluída com sucesso. Processados {Count} subdistritos", totalProcessed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização de subdistritos");
            throw;
        }
    }
}
