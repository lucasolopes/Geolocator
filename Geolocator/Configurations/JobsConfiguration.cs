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

            var ibgeJobKey = new JobKey("IbgeSyncJob");
            q.AddJob<IbgeSyncJob>(opts => opts.WithIdentity(ibgeJobKey));

            q.AddTrigger(opts => opts
                .ForJob(ibgeJobKey)
                .WithIdentity("IbgeSyncTrigger")
                .StartNow()
                .WithCronSchedule("0 0 1 1 * ?"));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }
}
