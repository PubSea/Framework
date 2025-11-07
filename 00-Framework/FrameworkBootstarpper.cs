using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.AspNetCore;
using Minio.AspNetCore.HealthChecks;
using PubSea.Framework.Data;
using PubSea.Framework.Events;
using PubSea.Framework.Grpc.Types;
using PubSea.Framework.Http;
using PubSea.Framework.Http.HealthCheck;
using PubSea.Framework.Mapping;
using PubSea.Framework.Middlewares;
using PubSea.Framework.Services.Abstractions;
using PubSea.Framework.Services.Dtos;
using PubSea.Framework.Services.Implementations;
using PubSea.Framework.Utility;

namespace PubSea.Framework.DomainModel;

public static class FrameworkBootstarpper
{
    /// <summary>
    /// Adds curl health check for the given endpoints
    /// </summary>
    /// <param name="healthChecksBuilder">health check builder</param>
    /// <param name="endpoints">endpoint</param>
    /// <returns></returns>
    public static IHealthChecksBuilder AddCurlHealthCheck(this IHealthChecksBuilder healthChecksBuilder,
        params HealthCheckEndpoint[] endpoints)
    {
        if (endpoints.Length == 0)
        {
            return healthChecksBuilder;
        }

        healthChecksBuilder.Services.AddHttpClient(CurlHealthCheck.HealthCheck, httpClient =>
        {
            httpClient.Timeout = TimeSpan.FromMilliseconds(200);
        });

        foreach (var endpoint in endpoints)
        {
            healthChecksBuilder.AddTypeActivatedCheck<CurlHealthCheck>(endpoint.Name, endpoint.Url);
        }

        return healthChecksBuilder;
    }

    /// <summary>
    /// Adds the default event dispatcher in appllication. You can register your dispatcher 
    /// in dotnet IoC container one by one as well.
    /// </summary>
    /// <param name="services"></param>
    public static IServiceCollection AddSeaEventDispatcher(this IServiceCollection services)
    {
        services.AddSnowflakeService();
        services.AddDateTimeService();
        services.TryAddScoped<IEventDispatcher, EventDispatcher>();

        var type = typeof(IEventHandler<>);
        var handlers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
            .Where(p => p.IsClass && IsAssignableToGenericType(p, type));

        foreach (var handler in handlers)
        {
            var events = handler.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .Select(i => i.GetGenericArguments()[0])
                .Distinct();

            foreach (var @event in events)
            {
                services.AddScoped(typeof(IEventHandler<>).MakeGenericType(@event), handler);
            }
        }

        return services;
    }

    /// <summary>
    /// Using of sea event dispatcher and its dependencies
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSeaEventDispatcher(this IApplicationBuilder app)
    {
        var snowflakeService = app.ApplicationServices.GetRequiredService<ISnowflakeService>();
        var dateTimeService = app.ApplicationServices.GetRequiredService<IDateTimeService>();

        SeaEventHelper.Init(snowflakeService, dateTimeService);
        return app;
    }

    /// <summary>
    /// Registers snowflake servcie in DI container.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config">SnowflakeOptions</param>
    public static void AddSnowflakeService(this IServiceCollection services, Action<SnowflakeOptions> config = default!)
    {
        var options = new SnowflakeOptions();
        config?.Invoke(options);
        services.TryAddSingleton<ISnowflakeService>(p => new SnowflakeService(options));
    }

    /// <summary>
    /// Registers hashid service in DI container.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    public static void AddHashIdService(this IServiceCollection services, Action<HashIdOptions> config = default!)
    {
        var options = new HashIdOptions();
        config?.Invoke(options);
        services.TryAddSingleton<IHashIdService>(p => new HashIdService(options));
    }

    /// <summary>
    /// Registers datetime service in DI container.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDateTimeService(this IServiceCollection services)
    {
        services.TryAddSingleton<IDateTimeService, DateTimeService>();
        return services;
    }

    /// <summary>
    /// Registers entity framework unit of work in DI container.
    /// </summary>
    /// <param name="services"></param>
    public static void AddEfUnitOfWork<T>(this IServiceCollection services)
        where T : DbContext
    {
        services.TryAddScoped<IUnitOfWork, EfUnitOfWork<T>>();
        services.TryAddScoped<IEfUnitOfWork, EfUnitOfWork<T>>();
    }

    /// <summary>
    /// This method ensures database has been created and is always updated.
    /// </summary>
    /// <typeparam name="T">T is type of DbContext of the project.</typeparam>
    /// <param name="app">app is ApplicationBuilder of the project.</param>
    public static void UseDatabase<T>(this IApplicationBuilder app)
        where T : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();
        try
        {
            using var context = scope.ServiceProvider.GetRequiredService<T>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IStartup>>();
            logger.LogWarning(ex, ex.Message);
        }
    }

    /// <summary>
    /// Registers ExceptionHandler middleware
    /// </summary>
    /// <param name="app"></param>
    public static void UseExecptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }

    /// <summary>
    /// Adds Sea's mapper to the project. It is powered by Mapster nuget package. 
    /// To configure mapping options, you can simply use SeaTypeAdapterConfig in the method or 
    /// inject it in another class and do your configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config">SeaTypeAdapterConfig</param>
    /// <returns></returns>
    public static IServiceCollection AddSeaMapper(this IServiceCollection services,
        Action<SeaTypeAdapterConfig> config = default!)
    {
        services.TryAddSingleton<SeaTypeAdapterConfig>();

        services.TryAddSingleton<ISeaMapper>(p =>
        {
            var adapterConfig = p.GetRequiredService<SeaTypeAdapterConfig>();
            config?.Invoke(adapterConfig);
            return new SeaMapper(p, adapterConfig);
        });

        return services;
    }

    /// <summary>
    /// Using of sea predefined mapper
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSeaMapper(this IApplicationBuilder app)
    {
        var config = app.ApplicationServices.GetRequiredService<SeaTypeAdapterConfig>();

        config.NewConfig<LocalDateTime, DateTime>()
          .MapWith(src => src.DateTime);

        config.NewConfig<DateTime, LocalDateTime>()
          .MapWith(src => new LocalDateTime(src));
        return app;
    }

    /// <summary>
    /// Registers file services in pipeline
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddSeaFileStore(this IServiceCollection services,
        Action<SeaFileStoreConfig> config)
    {
        SeaFileStoreConfig newConfig = new();
        config.Invoke(newConfig);

        services.TryAddSingleton(newConfig);

        services.TryAddScoped<ISeaFileStore, SeaFileStore>();

        services.AddMinio(options =>
        {
            var baseUrl = new Uri(newConfig.BaseUrl);
            var baseUrlHasSsl = baseUrl.Scheme == "https";

            options.Endpoint = baseUrl.Authority;
            options.AccessKey = newConfig.UserName;
            options.SecretKey = newConfig.Password;
            options.ConfigureClient(client =>
            {
                client.WithSSL(baseUrlHasSsl);
            });
        });

        services.AddHealthChecks()
            .AddMinio(sp =>
            {
                var factory = sp.GetRequiredService<Minio.AspNetCore.IMinioClientFactory>();
                return (MinioClient)factory.CreateClient();
            });

        return services;
    }

    /// <summary>
    /// Registers <see cref="HttpErrorLoggerMessageHandler"/> to services in order to be used in 
    /// <see cref="HttpClient"/> message handlers pipeline
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddHttpErrorLoggerMessageHandler(this IServiceCollection services)
    {
        services.AddScoped<HttpErrorLoggerMessageHandler>();

        return services;
    }

    static bool IsAssignableToGenericType(Type givenType, Type genericType)
    {
        var interfaceTypes = givenType.GetInterfaces();

        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        var baseType = givenType.BaseType!;
        if (baseType is null)
        {
            return false;
        }

        return IsAssignableToGenericType(baseType, genericType);
    }
}
