using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using Persistence;

namespace Geolocator.Configurations;

public static class DbContextConfiguration
{
    public static void ConfigureDbContext(this IServiceCollection services)
    {
        services.AddDbContext<GeolocatorDbContext>((serviceProvider, options) =>
        {
            string? connectionString = Environment.GetEnvironmentVariable("ConnectionString");

            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                MaxPoolSize = 100,
                MinPoolSize = 5,
                ConnectionIdleLifetime = 300,
                ConnectionPruningInterval = 60,
                Timeout = 60,
                CommandTimeout = 60,
                KeepAlive = 60,
                Pooling = true,
                Enlist = true,
                SslMode = SslMode.Prefer,
                AutoPrepareMinUsages = 5,
                MaxAutoPrepare = 20,
                ApplicationName = "Geolocator"
            };

            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger("DatabaseLogger");

            options.UseNpgsql(builder.ConnectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        5,
                        TimeSpan.FromSeconds(10),
                        null
                    );

                    npgsqlOptions.SetPostgresVersion(15, 0);
                    npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                })
                .LogTo(
                    log => logger.LogInformation(log.ToString()),
                    (eventId, logLevel) => logLevel >= LogLevel.Warning ||
                                           eventId.Id == RelationalEventId.CommandExecuted.Id &&
                                           logLevel >= LogLevel.Information
                )
                .EnableSensitiveDataLogging(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ==
                                            "Development")
                .EnableDetailedErrors(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                .UseLoggerFactory(loggerFactory);
        });

        services.AddHealthChecks()
            .AddCheck("Postgres Database", () =>
            {
                try
                {
                    using var conn = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionString"));
                    conn.Open();
                    using NpgsqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT 1";
                    cmd.ExecuteScalar();
                    return HealthCheckResult.Healthy();
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy(ex.Message);
                }
            }, new[] { "db", "postgres", "data" });
    }
}
