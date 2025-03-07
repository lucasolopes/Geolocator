using Application.Interfaces.Search;
using Elasticsearch.Options;
using Elasticsearch.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            ElasticsearchOptions options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ElasticsearchOptions>>().Value;
            ConnectionSettings? connectionSettings = new ConnectionSettings(new Uri(options.Uri))
                .DefaultMappingFor<Domain.Entities.Region>(m => m.IndexName(options.RegionIndexName))
                .DefaultMappingFor<Domain.Entities.State>(m => m.IndexName(options.StateIndexName))
                .DefaultMappingFor<Domain.Entities.Mesoregion>(m => m.IndexName(options.MesoregionIndexName))
                .DefaultMappingFor<Domain.Entities.MicroRegion>(m => m.IndexName(options.MicroRegionIndexName))
                .DefaultMappingFor<Domain.Entities.Municipality>(m => m.IndexName(options.MunicipalityIndexName))
                .DefaultMappingFor<Domain.Entities.Districts>(m => m.IndexName(options.DistrictIndexName))
                .DefaultMappingFor<Domain.Entities.SubDistricts>(m => m.IndexName(options.SubDistrictIndexName))
                .EnableDebugMode()
                .PrettyJson();

            return new ElasticClient(connectionSettings);
        });

        services.AddScoped<IElasticsearchService, ElasticsearchService>();
    }
}
