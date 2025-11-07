using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PubSea.Framework.Exceptions;
using PubSea.Messaging.EntityFramework.Configs;
using PubSea.Messaging.EntityFramework.RabbitMq.HostedServices;
using PubSea.Messaging.EntityFramework.RabbitMq.Services.Abstractions;
using PubSea.Messaging.EntityFramework.RabbitMq.Services.Implementations;
using PubSea.Messaging.EntityFramework.Services.Abstractions;

namespace PubSea.Messaging.EntityFramework.RabbitMq.Configs;

public static class SeaRabbitMqMessagingConfigExtensions
{
    /// <summary>
    /// Adds RabbitMq transport layer to messaging system.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="rabbitMqConfig"></param>
    /// <returns></returns>
    /// <exception cref="SeaException"></exception>
    public static SeaMessagingConfig UseRabbitMqBroker(this SeaMessagingConfig config,
        Action<RabbitMqConfig> rabbitMqConfig)
    {
        var newRabbitMqConfig = new RabbitMqConfig();
        rabbitMqConfig.Invoke(newRabbitMqConfig);

        if (config.BrokerType != BrokerType.None)
        {
            throw new SeaException("Only one transporter can be specified(like Kafka or RabbitMq)",
                SeaException.INTERNAL_ERROR_CODE, ExceptionStatus.Internal);
        }
        config.BrokerType = BrokerType.RabbitMq;

        if (string.IsNullOrWhiteSpace(newRabbitMqConfig.ClientId))
        {
            var methodName = nameof(UseRabbitMqBroker);
            throw new SeaException($"ClientId MUST be specified. It can be set in ${methodName} method");
        }

        config.Services.TryAddSingleton<IRabbitMqClient, RabbitMqClient>();

        config.Services.TryAddSingleton(newRabbitMqConfig);
        config.Services.TryAddSingleton<IBrokerService, RabbitMqBrokerService>();

        config.Services.AddHostedService<RabbitMqMainConsumer>();

        return config;
    }
}
