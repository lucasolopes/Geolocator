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

            var jobKey = new JobKey("IbgeSyncJob");
            q.AddJob<IbgeSyncJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("IbgeSyncTrigger")
                .StartNow()
                .WithCronSchedule("0 0 1 * * ?"));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }
}
