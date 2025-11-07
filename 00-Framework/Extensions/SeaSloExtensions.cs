using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PubSea.Framework.Middlewares;
using PubSea.Framework.Services.Abstractions;
using PubSea.Framework.Services.Implementations;
using StackExchange.Redis;

namespace PubSea.Framework.Extensions;

public static class SeaSloExtensions
{
    internal const string LOGOUT_CONNECTION_MULTIPLEXER = "logout";
    internal const byte LOGOUT_CONNECTION_MULTIPLEXER_DB = 14;
    internal const string LOGOUT_CONNECTION_MULTIPLEXER_PREFIX = "logout__";

    /// <summary>
    /// Registers slo services to pipeline and gives power of pushing logout event to Redis 
    /// using <see cref="ILogoutPusher" /> service. This service is injectable. To use slo services 
    /// <see cref="SeaCachingExtensions.AddSeaRedisHybridCache(IServiceCollection, Action{RedisHybridOptions})"/> is needed to 
    /// be registered
    /// </summary>
    /// <param name="services"></param>
    /// <param name="redisConnectionString"></param>
    /// <returns></returns>
    public static IServiceCollection AddSeaIdentityProviderSlo(this IServiceCollection services,
        string redisConnectionString)
    {
        services.TryAddKeyedSingleton<IConnectionMultiplexer>(LOGOUT_CONNECTION_MULTIPLEXER, (sp, key) =>
        {
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });

        services.TryAddScoped<ILogoutPusher, LogoutPusher>();

        return services;
    }

    /// <summary>
    /// Adds sigle logout to pipeline. If a session has expired this will catch it and does not 
    /// allow to coninue request and of course remove current session cookies
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSeaIdentityProviderSlo(this IApplicationBuilder app)
    {
        app.UseMiddleware<TerminatedSessionsMiddleware>();

        return app;
    }
}