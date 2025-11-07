using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PubSea.Framework.Exceptions;
using PubSea.Framework.Services.Abstractions;
using PubSea.Framework.Services.Implementations;

namespace PubSea.Framework.Extensions;

public static class SeaCachingExtensions
{
    public static IServiceCollection AddSeaRedisHybridCache(this IServiceCollection services,
        Action<RedisHybridOptions> options)
    {
        services.AddHybridCache(hco =>
        {
            RedisHybridOptions opt = new(hco);
            options.Invoke(opt);
        });

        services.AddStackExchangeRedisCache(r =>
        {
            RedisHybridOptions opt = new(r);
            options.Invoke(opt);
        });

        services.TryAddSingleton<ISeaConnectionMultiplexer>(p =>
        {
            var r = new RedisCacheOptions();
            RedisHybridOptions opt = new(r);
            options.Invoke(opt);

            if (string.IsNullOrWhiteSpace(r.Configuration))
            {
                throw new SeaException("Redis conneciton string is not specified",
                    SeaException.INTERNAL_ERROR_CODE, ExceptionStatus.Internal);
            }

            return new SeaConnectionMultiplexer(r.Configuration);
        });

        return services;
    }
}

public sealed class RedisHybridOptions
{
    private readonly HybridCacheOptions? _hco;
    private readonly RedisCacheOptions? _ro;

    public RedisHybridOptions(HybridCacheOptions hco)
    {
        _hco = hco;
    }

    public RedisHybridOptions(RedisCacheOptions ro)
    {
        _ro = ro;
    }

    public void ConfigureHybridCache(Action<HybridCacheOptions> options)
    {
        if (_hco is null)
        {
            return;
        }

        options.Invoke(_hco);
    }

    public void ConfigureRedis(Action<RedisCacheOptions> options)
    {
        if (_ro is null)
        {
            return;
        }

        options.Invoke(_ro);
    }
}
