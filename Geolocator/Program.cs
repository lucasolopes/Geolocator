using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces.Search;
using Elasticsearch.Extensions;
using Geolocator.Configurations;
using Microsoft.EntityFrameworkCore;
using Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext();

builder.Services.ConfigureCors();

builder.Services.AddQuartzJobs();

builder.Services.AddElasticsearch(builder.Configuration);

builder.Services.ConfigureServices(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;

        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

app.UseCors("AllowLocalhostAndZero");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (IServiceScope scope = app.Services.CreateScope())
{
    try
    {
        GeolocatorDbContext db = scope.ServiceProvider.GetRequiredService<GeolocatorDbContext>();
        await db.Database.MigrateAsync();

        IElasticsearchService elasticService = scope.ServiceProvider.GetRequiredService<IElasticsearchService>();
        await elasticService.CreateIndicesIfNotExistAsync();
    }
    catch (Exception ex)
    {
        ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro durante a inicialização da aplicação");
    }
}

await app.RunAsync();
