using Application.Commands.ElasticsearchSync;
using Application.Commands.IbgeSync;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BackgroundJobs.Jobs;

[DisallowConcurrentExecution]
public class IbgeSyncJob : IJob
{
    private readonly ILogger<IbgeSyncJob> _logger;
    private readonly IMediator _mediator;

    public IbgeSyncJob(IMediator mediator, ILogger<IbgeSyncJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Iniciando sincronização IBGE...");

        try
        {
            _logger.LogInformation("Iniciando sincronização de regiões");
            await _mediator.Send(new SyncRegionsCommand());
            _logger.LogInformation("Sincronização de regiões concluída");

            _logger.LogInformation("Iniciando sincronização de estados");
            await _mediator.Send(new SyncStatesCommand());
            _logger.LogInformation("Sincronização de estados concluída");

            _logger.LogInformation("Iniciando sincronização de mesorregiões");
            await _mediator.Send(new SyncMesoregionsCommand());
            _logger.LogInformation("Sincronização de mesorregiões concluída");

            _logger.LogInformation("Iniciando sincronização de microrregiões");
            await _mediator.Send(new SyncMicroregionsCommand());
            _logger.LogInformation("Sincronização de microrregiões concluída");

            _logger.LogInformation("Iniciando sincronização de municípios");
            await _mediator.Send(new SyncMunicipalitiesCommand());
            _logger.LogInformation("Sincronização de municípios concluída");

            _logger.LogInformation("Iniciando sincronização de distritos");
            await _mediator.Send(new SyncDistrictsCommand());
            _logger.LogInformation("Sincronização de distritos concluída");

            _logger.LogInformation("Iniciando sincronização de subdistritos");
            await _mediator.Send(new SyncSubDistrictsCommand());
            _logger.LogInformation("Sincronização de subdistritos concluída");

            _logger.LogInformation("Iniciando sincronização com o Elasticsearch");
            SyncElasticsearchResult elasticsearchResult = await _mediator.Send(new SyncElasticsearchCommand());
            _logger.LogInformation("Sincronização com o Elasticsearch concluída. Resultado: {Result}",
                elasticsearchResult.Success ? "Sucesso" : "Falha parcial");

            _logger.LogInformation("Sincronização IBGE e Elasticsearch concluída com sucesso.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a sincronização IBGE: {Message}", ex.Message);
            throw;
        }
    }
}
