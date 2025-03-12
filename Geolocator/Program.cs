using System.Text.Json.Serialization;
using System.Text.Json;
using Application.Interfaces.Search;
using Elasticsearch.Extensions;
using Geolocator.Configurations;
using Microsoft.EntityFrameworkCore;
using Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

app.UseCors("AllowLocalhostAndZero");

// Configure the HTTP request pipeline.
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
        // Migra o banco de dados PostgreSQL
        GeolocatorDbContext db = scope.ServiceProvider.GetRequiredService<GeolocatorDbContext>();
        await db.Database.MigrateAsync();

        // Inicializa os índices do Elasticsearch
        IElasticsearchService elasticService = scope.ServiceProvider.GetRequiredService<Application.Interfaces.Search.IElasticsearchService>();
        await elasticService.CreateIndicesIfNotExistAsync();
    }
    catch (Exception ex)
    {
        ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro durante a inicialização da aplicação");
    }
}

await app.RunAsync();
