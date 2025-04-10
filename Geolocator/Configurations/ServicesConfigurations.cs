﻿using Application.Interfaces.Search;
using Elasticsearch.DTOs;
using Elasticsearch.Options;
using Elasticsearch.Services;
using ExternalServices;
using Microsoft.Extensions.Options;
using Nest;
using Scrutor;

namespace Geolocator.Configurations;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Scan(scan => scan
            .FromAssemblies(
                AssemblyReference.Assembly,
                Persistence.AssemblyReference.Assembly)
            .AddClasses()
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        services.AddHttpClient();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            Application.AssemblyReference.Assembly));

        services.Configure<ElasticsearchOptions>(options =>
        {
            options.Uri = configuration["ElasticsearchUri"] ?? "http://localhost:9200";
        });

        services.AddSingleton<IElasticClient>(sp =>
        {
            ElasticsearchOptions options = sp.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;
            ConnectionSettings? connectionSettings = new ConnectionSettings(new Uri(options.Uri))
                .DefaultMappingFor<RegionDto>(m => m.IndexName(options.RegionIndexName))
                .DefaultMappingFor<StateDto>(m => m.IndexName(options.StateIndexName))
                .DefaultMappingFor<MesoregionDto>(m => m.IndexName(options.MesoregionIndexName))
                .DefaultMappingFor<MicroRegionDto>(m => m.IndexName(options.MicroRegionIndexName))
                .DefaultMappingFor<MunicipalityDto>(m => m.IndexName(options.MunicipalityIndexName))
                .DefaultMappingFor<DistrictsDto>(m => m.IndexName(options.DistrictIndexName))
                .DefaultMappingFor<SubDistrictsDto>(m => m.IndexName(options.SubDistrictIndexName))
                .DisableDirectStreaming() // Facilita a depuração
                .EnableDebugMode()
                .PrettyJson()
                .RequestTimeout(TimeSpan.FromMinutes(2)) // Aumentar timeout
                .MaximumRetries(5) // Adicionar retries
                .MaxRetryTimeout(TimeSpan.FromMinutes(5));

            return new ElasticClient(connectionSettings);
        });

        services.AddTransient<IElasticsearchService, ElasticsearchService>();
    }
}
