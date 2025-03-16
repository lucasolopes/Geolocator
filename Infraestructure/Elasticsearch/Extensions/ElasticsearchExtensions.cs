using Application.Interfaces.Search;
using Domain.Entities;
using Elasticsearch.Options;
using Elasticsearch.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;

namespace Elasticsearch.Extensions;

public static class ElasticsearchExtensions
{
    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElasticsearchOptions>(options =>
        {
            options.Uri = configuration["ElasticsearchUri"] ?? "http://localhost:9200";
        });

        services.AddSingleton<IElasticClient>(sp =>
        {
            ElasticsearchOptions options = sp.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;
            ConnectionSettings? connectionSettings = new ConnectionSettings(new Uri(options.Uri))
                .DefaultMappingFor<Region>(m => m.IndexName(options.RegionIndexName))
                .DefaultMappingFor<State>(m => m.IndexName(options.StateIndexName))
                .DefaultMappingFor<Mesoregion>(m => m.IndexName(options.MesoregionIndexName))
                .DefaultMappingFor<MicroRegion>(m => m.IndexName(options.MicroRegionIndexName))
                .DefaultMappingFor<Municipality>(m => m.IndexName(options.MunicipalityIndexName))
                .DefaultMappingFor<Districts>(m => m.IndexName(options.DistrictIndexName))
                .DefaultMappingFor<SubDistricts>(m => m.IndexName(options.SubDistrictIndexName))
                .EnableDebugMode()
                .PrettyJson();

            return new ElasticClient(connectionSettings);
        });

        services.AddScoped<IElasticsearchService, ElasticsearchService>();
    }
}
