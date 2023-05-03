namespace Broker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var settingsSection = configuration.GetSection(ApplicationSettings.SectionName);
        services.Configure<ApplicationSettings>(settingsSection);

        var assembly = typeof(AssemblyMarker).Assembly;

        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(assembly);
        });

        AddMapster(services);

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblies(new[] { assembly });

        services.AddApplicationServices();

        return services;
    }

    private static IServiceCollection AddMapster(IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(AssemblyMarker).Assembly);
        services.AddSingleton(config);

        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IExchangeRatesProvider, ExchangeRatesProvider>();

        return services;
    }
}