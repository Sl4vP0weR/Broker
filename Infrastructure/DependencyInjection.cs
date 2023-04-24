namespace Broker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var settingsSection = configuration.GetSection(RedisSettings.SectionName);
        services.Configure<RedisSettings>(settingsSection);

        services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(provider =>
        {
            var settings = provider.GetService<IOptions<RedisSettings>>();
            var result = new List<RedisConfiguration>() { settings.Value };

            return result;
        });

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(AssemblyMarker).Assembly);

        services.AddInfrastructureServices();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IExchangeRatesCache, ExchangeRatesCache>();
        services.AddScoped<IExchangeRatesAPI, FixerExchangeRatesAPI>();

        return services;
    }

    /// <summary>
    /// Add StackExchange.Redis with its serialization provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="redisConfiguration">The redis configration.</param>
    /// <typeparam name="T">The typof of serializer. <see cref="ISerializer" />.</typeparam>
    public static IServiceCollection AddStackExchangeRedisExtensions<T>(
            this IServiceCollection services,
            Func<IServiceProvider, IEnumerable<RedisConfiguration>> redisConfigurationFactory)
            where T : class, ISerializer
    {
        services.AddSingleton<IRedisClientFactory, RedisClientFactory>();
        services.AddSingleton<ISerializer, T>();

        services.AddSingleton((provider) => provider
            .GetRequiredService<IRedisClientFactory>()
            .GetDefaultRedisClient());

        services.AddSingleton((provider) => provider
            .GetRequiredService<IRedisClientFactory>()
            .GetDefaultRedisClient()
            .GetDefaultDatabase());

        services.AddSingleton(redisConfigurationFactory);

        return services;
    }
}