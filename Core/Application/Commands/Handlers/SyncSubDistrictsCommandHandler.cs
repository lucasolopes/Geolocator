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
            List<SubDistricts> ibgeSubDistricts = await _ibgeService.GetSubDistrictsAsync();

            HashSet<long> subDistrictsIds = new(await _subDistrictsRepository.GetAllIdsAsync());

            var newSubDistricts = ibgeSubDistricts.Where(s => !subDistrictsIds.Contains(s.Id)).ToList();

            if (newSubDistricts.Any())
            {
                _logger.LogInformation("Encontrados {Count} novos subdistritos", newSubDistricts.Count);
                await _subDistrictsRepository.AddRangeAsync(newSubDistricts);
            }

            await _subDistrictsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sincronização de subdistritos concluída com sucesso. Processados {Count} subdistritos", newSubDistricts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização de subdistritos");
            throw;
        }
    }
}
