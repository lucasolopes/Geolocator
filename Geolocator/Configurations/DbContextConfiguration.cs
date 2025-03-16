using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Persistence;
using System;

namespace Geolocator.Configurations;

public static class DbContextConfiguration
{
    public static void ConfigureDbContext(this IServiceCollection services)
    {
        services.AddDbContext<GeolocatorDbContext>((serviceProvider, options) =>
        {
            string? connectionString = Environment.GetEnvironmentVariable("ConnectionString");

            // Adicionando parâmetros de pooling diretamente na string de conexão
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
                Enlist = true,                    // Participação em transações distribuídas
                SslMode = SslMode.Prefer,         // Preferência por SSL, se disponível
                AutoPrepareMinUsages = 5,         // Prepara automaticamente statements usados mais de 5 vezes
                MaxAutoPrepare = 20,              // Máximo de statements preparados automaticamente
                ApplicationName = "Geolocator"    // Nome da aplicação para identificação no servidor
            };

            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger("DatabaseLogger");

            options.UseNpgsql(builder.ConnectionString, npgsqlOptions =>
                {
                    // Configurações de Resiliência
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null
                    );

                    // Configurações de Desempenho
                    npgsqlOptions.SetPostgresVersion(15, 0);
                    npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                })
                .LogTo(
                    log => logger.LogInformation(log.ToString()),
                    (eventId, logLevel) => logLevel >= LogLevel.Warning ||
                                          eventId.Id == Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted.Id &&
                                          logLevel >= LogLevel.Information
                )
                .EnableSensitiveDataLogging(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                .EnableDetailedErrors(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                .UseLoggerFactory(loggerFactory);
        }, ServiceLifetime.Scoped);

        // Adiciona uma verificação básica de saúde para o banco
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
                    return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
                }
                catch (Exception ex)
                {
                    return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy(ex.Message);
                }
            }, tags: new[] { "db", "postgres", "data" });
    }
}
