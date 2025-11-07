using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PubSea.Messaging.EntityFramework.Kafka.Configs;

namespace PubSea.Messaging.EntityFramework.Kafka.HostedServices;

internal sealed class OutboxMessageTopicsInitializer : BackgroundService
{
    private readonly ILogger<OutboxMessageTopicsInitializer> _logger;
    private readonly KafkaConfig _config;
    private readonly AdminClientConfig _adminClientConfig;

    public OutboxMessageTopicsInitializer(ILogger<OutboxMessageTopicsInitializer> logger, KafkaConfig config)
    {
        _logger = logger;
        _config = config;
        _adminClientConfig = new AdminClientConfig
        {
            BootstrapServers = _config.ConnectionString,
            ClientId = _config.ClientId,
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_config.TopicName))
            {
                return;
            }

            using var adminClient = new AdminClientBuilder(_adminClientConfig).Build();
            var topicMetadata = adminClient.GetMetadata(TimeSpan.FromMinutes(1)).Topics;

            await CreateTopic(adminClient, topicMetadata, _config.TopicName, _config.Partitions);
        }
        catch (CreateTopicsException ex)
        {
            _logger.LogCritical(ex, "Something went wrong in creating {TopicName} topic", _config.TopicName);
        }
    }

    private async Task CreateTopic(IAdminClient adminClient, List<TopicMetadata> metadata,
        string topicName, byte partitions)
    {
        var topicExists = metadata.Any(t => t.Topic == topicName);

        if (topicExists)
        {
            return;
        }

        await adminClient.CreateTopicsAsync(
            [
                new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = partitions,
                },
            ]);

        _logger.LogInformation("Topic {TopicName} created successfully", _config.TopicName);
    }
}
