using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PubSea.Messaging.EntityFramework.Kafka.Configs;
using PubSea.Messaging.EntityFramework.Kafka.Serializers;
using PubSea.Messaging.EntityFramework.Models;
using PubSea.Messaging.EntityFramework.Services.Abstractions;
using System.Diagnostics;

namespace PubSea.Messaging.EntityFramework.Kafka.HostedServices;

internal sealed class KafkaMainConsumer : BackgroundService
{
    private readonly byte _commitIntervalCount = 100;
    private readonly TimeSpan _commitIntervalTime = TimeSpan.FromMinutes(5);

    private readonly ILogger<KafkaMainConsumer> _logger;
    private readonly KafkaConfig _kafkaConfig;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerConfig _consumerConfig;
    private readonly IConsumer<long, OutboxMessage> _consumer;

    public KafkaMainConsumer(ILogger<KafkaMainConsumer> logger, KafkaConfig kafkaConfig,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _kafkaConfig = kafkaConfig;
        _serviceProvider = serviceProvider;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaConfig.ConnectionString,
            ClientId = kafkaConfig.ClientId,
            GroupId = kafkaConfig.ConsumerGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };
        _consumer = new ConsumerBuilder<long, OutboxMessage>(_consumerConfig)
            .SetKeyDeserializer(new MessageKeyDeserializer())
            .SetValueDeserializer(new MessageValueDeserializer())
            .Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Factory.StartNew(async () =>
        {
            await Process(stoppingToken);
        }, TaskCreationOptions.LongRunning);
    }

    private async Task Process(CancellationToken stoppingToken)
    {
        _logger.LogInformation("KafkaMainConsumer started working {StartDate}", DateTime.UtcNow);

        if (_kafkaConfig.ConsumingTopicNames.Count == 0)
        {
            _logger.LogInformation("There is not ConsumingTopicNames in config so KafkaMainConsumer stopped working {StartDate}", DateTime.UtcNow);
            return;
        }

        _consumer.Subscribe(_kafkaConfig.ConsumingTopicNames);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var startTime = Stopwatch.GetTimestamp();
                var consumeResult = _consumer.Consume(stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                var mainConsumer = scope.ServiceProvider.GetRequiredService<IMainConsumer>();
                await mainConsumer.Consume(scope, consumeResult.Message.Value, stoppingToken);

                CommitPeriodically(startTime, consumeResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wront in KafkaMainConsumer");
            }
        }
    }

    private void CommitPeriodically(long startTime, ConsumeResult<long, OutboxMessage> consumeResult)
    {
        if ((consumeResult.Offset % _commitIntervalCount == 0) ||
            (Stopwatch.GetElapsedTime(startTime) >= _commitIntervalTime))
        {
            _consumer.Commit(consumeResult);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogCritical("KafkaMainConsumer stopped working {StopDate}", DateTime.UtcNow);
        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();

        base.Dispose();
    }
}
