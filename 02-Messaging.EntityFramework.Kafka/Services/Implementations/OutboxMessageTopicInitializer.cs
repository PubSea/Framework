using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using PubSea.Messaging.EntityFramework.Kafka.Configs;

namespace PubSea.Messaging.EntityFramework.Kafka.Services.Implementations;

internal sealed class OutboxMessageTopicInitializer
{
    private readonly KafkaConfig _config;
    private readonly ILogger<OutboxMessageTopicInitializer> _logger;
    private readonly AdminClientConfig _adminClientConfig;

    public OutboxMessageTopicInitializer(KafkaConfig config, ILogger<OutboxMessageTopicInitializer> logger)
    {
        _config = config;
        _logger = logger;
        _adminClientConfig = new AdminClientConfig
        {
            BootstrapServers = _config.ConnectionString,
            ClientId = _config.ClientId,
        };
    }

    public async Task Init()
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