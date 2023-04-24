namespace Broker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApplicationSettings>(configuration.GetSection(nameof(ApplicationSettings)));

        var assembly = typeof(AssemblyMarker).Assembly;

        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(assembly);
        });

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(assembly, typeof(Domain.AssemblyMarker).Assembly);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddValidatorsFromAssembly(assembly);

        services.AddApplicationServices();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IExchangeRatesProvider, ExchangeRatesProvider>();

        return services;
    }
}