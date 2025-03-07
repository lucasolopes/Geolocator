using Scrutor;

namespace Geolocator.Configurations;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblies(
                ExternalServices.AssemblyReference.Assembly,
                Persistence.AssemblyReference.Assembly)
            .AddClasses()
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddHttpClient();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            Application.AssemblyReference.Assembly));
    }
}
