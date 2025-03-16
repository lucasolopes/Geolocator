using Application.Commands.IbgeSync;
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
    private readonly IMicroRegionRepository _microRegionRepository;
    private readonly IMunicipalityRepository _municipalityRepository;

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
            List<Municipality> ibgeMunicipalities = await _ibgeService.GetMunicipalitiesAsync();

            HashSet<long> municipalitiesIds = new(await _municipalityRepository.GetAllIdsAsync());

            var newMunicipalities = ibgeMunicipalities.Where(m => !municipalitiesIds.Contains(m.Id)).ToList();

            if (newMunicipalities.Any())
            {
                _logger.LogInformation("Encontrados {Count} novos municípios", newMunicipalities.Count);
                await _municipalityRepository.AddRangeAsync(newMunicipalities);
            }

            await _municipalityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sincronização de municípios concluída com sucesso. Processados {Count} municípios",
                newMunicipalities.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização de municípios");
            throw;
        }
    }
}
