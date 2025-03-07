
using Application.Commands.IbgeSync;
using Domain.Entities;
using MediatR;
using Quartz;

namespace BackgroundJobs.Jobs;

[DisallowConcurrentExecution]
public class IbgeSyncJob : IJob
{
    private readonly IMediator _mediator;

    public IbgeSyncJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Starting IBGE synchronization...");
            
        try
        {
            // Sincroniza regiões
            await _mediator.Send(new SyncRegionsCommand());
               /* 
            // Sincroniza estados
            await _mediator.Send(new SyncStatesCommand());
                
            // Sincroniza mesorregiões
            await _mediator.Send(new SyncMesoregionsCommand());
                
            // Sincroniza microrregiões
            await _mediator.Send(new SyncMicroregionsCommand());
                
            // Sincroniza municípios
            await _mediator.Send(new SyncMunicipalitiesCommand());
                
            // Sincroniza distritos
            await _mediator.Send(new SyncDistrictsCommand());
                
            // Sincroniza subdistritos
            await _mediator.Send(new SyncSubDistrictsCommand());
                */
            Console.WriteLine("IBGE synchronization completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during IBGE synchronization: {ex.Message}");
            throw;
        }
    }
}
