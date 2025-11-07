using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using PubSea.Messaging.EntityFramework.Models;
using PubSea.Messaging.EntityFramework.RabbitMq.Configs;
using PubSea.Messaging.EntityFramework.RabbitMq.Services.Abstractions;
using PubSea.Messaging.EntityFramework.Services.Abstractions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PubSea.Messaging.EntityFramework.RabbitMq.HostedServices;

internal sealed class RabbitMqMainConsumer : BackgroundService
{
    private readonly ILogger<RabbitMqMainConsumer> _logger;
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly RabbitMqConfig _config;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqMainConsumer(ILogger<RabbitMqMainConsumer> logger, IRabbitMqClient rabbitMqClient,
        RabbitMqConfig config, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _rabbitMqClient = rabbitMqClient;
        _config = config;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMqMainConsumer started working {StartDate}", DateTime.UtcNow);

        foreach (var (typeName, _) in MessageContracts.Types)
        {
            try
            {
                var channel = await _rabbitMqClient.GetNewChannel(stoppingToken);
                var queue = $"{_config.ClientId}__{typeName}";
                await DeclareQueue(channel, typeName, queue, stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += Consume;

                _ = await channel.BasicConsumeAsync(queue, autoAck: false, consumer, stoppingToken);
            }
            catch (OperationInterruptedException ex)
            {
                _logger.LogError(ex,
                    "The AMQP operation was interrupted. It might occur if queue does not exist {Queue} {DateTime}",
                    typeName, DateTime.UtcNow);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogCritical("RabbitMqMainConsumer stopped working {StopDate}", DateTime.UtcNow);
        return base.StopAsync(cancellationToken);
    }

    private async Task Consume(object basicConsumer, BasicDeliverEventArgs args)
    {
        var channel = ((AsyncEventingBasicConsumer)basicConsumer).Channel;
        var body = args.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);
        var message = JsonSerializer.Deserialize<OutboxMessage>(json, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        using var scope = _serviceProvider.CreateScope();
        var mainConsumer = scope.ServiceProvider.GetRequiredService<IMainConsumer>();
        await mainConsumer.Consume(scope, message!);

        await channel.BasicAckAsync(args.DeliveryTag, false);
    }

    private static async Task DeclareQueue(IChannel channel, string exchange, string queue, CancellationToken ct)
    {
        await channel.QueueDeclareAsync(queue, durable: true, exclusive: false,
            autoDelete: false, cancellationToken: ct);

        await channel.QueueBindAsync(queue, exchange, routingKey: string.Empty, cancellationToken: ct);
    }
}
