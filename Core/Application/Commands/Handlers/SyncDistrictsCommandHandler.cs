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
            List<Districts> ibgeDistricts = await _ibgeService.GetDistrictsAsync();

            HashSet<long> districtsIds = await _districtsRepository.GetAllIdsAsync();

            var newDistricts = ibgeDistricts.Where(d => !districtsIds.Contains(d.Id)).ToList();

            if (newDistricts.Any())
            {
                _logger.LogInformation("Encontrados {Count} novos distritos", newDistricts.Count);
                await _districtsRepository.AddRangeAsync(newDistricts);
            }

            await _districtsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sincronização de distritos concluída com sucesso. Processados {Count} distritos", newDistricts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização de distritos");
            throw;
        }
    }
}
