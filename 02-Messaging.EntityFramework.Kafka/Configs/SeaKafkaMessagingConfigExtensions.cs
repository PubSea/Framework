using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using PubSea.Framework.Exceptions;
using PubSea.Messaging.EntityFramework.Configs;
using PubSea.Messaging.EntityFramework.Kafka.HostedServices;
using PubSea.Messaging.EntityFramework.Kafka.Services.Implementations;
using PubSea.Messaging.EntityFramework.Services.Abstractions;

namespace PubSea.Messaging.EntityFramework.Kafka.Configs;

public static class SeaKafkaMessagingConfigExtensions
{
    /// <summary>
    /// Adds Kafka transport layer to messaging system.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="kafkaConfig"></param>
    /// <returns></returns>
    /// <exception cref="SeaException"></exception>
    public static SeaMessagingConfig UseKafkaBroker(this SeaMessagingConfig config,
        Action<KafkaConfig> kafkaConfig)
    {
        var newKafkaConfig = new KafkaConfig();
        kafkaConfig.Invoke(newKafkaConfig);


        if (config.BrokerType != BrokerType.None)
        {
            throw new SeaException("Only one transporter can be specified(like Kafka or RabbitMq)",
                SeaException.INTERNAL_ERROR_CODE, ExceptionStatus.Internal);
        }

        config.BrokerType = BrokerType.Kafka;

        config.Services.TryAddSingleton(newKafkaConfig);
        config.Services.TryAddSingleton<IBrokerService, KafkaBrokerService>();
        config.Services.AddHostedService<OutboxMessageTopicsInitializer>();

        AddConsumers(config.Services, newKafkaConfig);

        return config;
    }

    private static void AddConsumers(IServiceCollection services, KafkaConfig config)
    {
        if (config.ConsumingTopicNames.Count == 0 || string.IsNullOrWhiteSpace(config.ConsumerGroupId))
        {
            return;
        }

        var consumersCounter = config.ConcurrentConsumers;
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, KafkaMainConsumer>());
        while (--consumersCounter > 0)
        {
            services.Add(ServiceDescriptor.Singleton<IHostedService, KafkaMainConsumer>());
        }
    }
}
