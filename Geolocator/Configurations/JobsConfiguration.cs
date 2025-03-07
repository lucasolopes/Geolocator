using BackgroundJobs.Jobs;
using Quartz;

namespace Geolocator.Configurations;

public static class JobsConfiguration
{
    public static void AddQuartzJobs(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            // Job para sincronização com o IBGE e Elasticsearch
            var ibgeJobKey = new JobKey("IbgeSyncJob");
            q.AddJob<IbgeSyncJob>(opts => opts.WithIdentity(ibgeJobKey));

            q.AddTrigger(opts => opts
                .ForJob(ibgeJobKey)
                .WithIdentity("IbgeSyncTrigger")
                .StartNow()
                .WithCronSchedule("0 0 1 1 * ?")); // Executar às 00:00 do primeiro dia de cada mês
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }
}
