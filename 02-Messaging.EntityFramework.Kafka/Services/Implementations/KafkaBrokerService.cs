using Confluent.Kafka;
using PubSea.Framework.Exceptions;
using PubSea.Messaging.EntityFramework.Kafka.Configs;
using PubSea.Messaging.EntityFramework.Kafka.Serializers;
using PubSea.Messaging.EntityFramework.Models;
using PubSea.Messaging.EntityFramework.Services.Abstractions;
using PubSea.Framework.Utility;
using System.Text;

namespace PubSea.Messaging.EntityFramework.Kafka.Services.Implementations;

internal sealed class KafkaBrokerService : IBrokerService
{
    private readonly KafkaConfig _config;
    private readonly ProducerConfig _producerConfig;
    private readonly IProducer<long, OutboxMessage> _producer;

    public KafkaBrokerService(KafkaConfig config)
    {
        _config = config;
        _producerConfig = new()
        {
            BootstrapServers = _config.ConnectionString,
            ClientId = _config.ClientId,
            AllowAutoCreateTopics = false,
        };
        _producer = new ProducerBuilder<long, OutboxMessage>(_producerConfig)
            .SetKeySerializer(new MessageKeySerializer())
            .SetValueSerializer(new MessageValueSerializer())
            .Build();
    }

    async Task IBrokerService.TransportMessages(ICollection<OutboxMessage> messages, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_config.TopicName))
        {
            var methodName = nameof(SeaKafkaMessagingConfigExtensions.UseKafkaBroker);
            throw new SeaException(
                $"Outbox topic name MUST be set in {methodName} method",
                SeaException.INTERNAL_ERROR_CODE, ExceptionStatus.Internal);
        }

        var tasks = new List<Task<DeliveryResult<long, OutboxMessage>>>(messages.Count);
        foreach (var message in messages)
        {
            var dto = new Message<long, OutboxMessage>
            {
                Key = message.Id,
                Value = message,
            };

            var partitionNumber = GeneratePartitionKey(message);
            var topicPartition = new TopicPartition(_config.TopicName, partitionNumber);
            tasks.Add(_producer.ProduceAsync(topicPartition, dto, ct));
        }

        await TaskWrapper.WrapWhenAll(tasks);

        if (tasks.Any(t => !t.IsCompletedSuccessfully))
        {
            var ex = tasks.First(t => !t.IsCompletedSuccessfully).Exception;
            throw new SeaException("Something went wront in publishing message",
                SeaException.INTERNAL_ERROR_CODE, ExceptionStatus.Internal, ex!);
        }
    }

    private byte GeneratePartitionKey(OutboxMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.PrioritizerKey))
        {
            return (byte)(message.Id % _config.Partitions);
        }


        var asciiSum = Encoding.ASCII.GetBytes(message.PrioritizerKey).Sum(b => b);
        return (byte)(asciiSum % _config.Partitions);
    }
}
