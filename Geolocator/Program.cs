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
    IServiceProvider services = scope.ServiceProvider;
    ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Tentando aplicar migrações ao banco de dados...");
        GeolocatorDbContext context = services.GetRequiredService<GeolocatorDbContext>();
        context.Database.Migrate();
        logger.LogInformation("Migrações aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocorreu um erro durante a aplicação das migrações");
    }
}

await app.RunAsync();
