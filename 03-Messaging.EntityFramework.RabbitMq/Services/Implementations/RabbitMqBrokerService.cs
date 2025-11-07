using RabbitMQ.Client;
using PubSea.Messaging.EntityFramework.Models;
using PubSea.Messaging.EntityFramework.RabbitMq.Services.Abstractions;
using PubSea.Messaging.EntityFramework.Services.Abstractions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PubSea.Messaging.EntityFramework.RabbitMq.Services.Implementations;

internal sealed class RabbitMqBrokerService : IBrokerService
{
    private static readonly Dictionary<string, bool> _exchangeStates = [];

    private readonly IRabbitMqClient _rabbitMqClient;

    public RabbitMqBrokerService(IRabbitMqClient rabbitMqClient)
    {
        _rabbitMqClient = rabbitMqClient;
    }

    async Task IBrokerService.TransportMessages(ICollection<OutboxMessage> messages, CancellationToken ct)
    {
        foreach (var message in messages)
        {
            await Transport(message, ct);
        }
    }

    private async Task Transport(OutboxMessage message, CancellationToken ct)
    {
        var channel = await _rabbitMqClient.GetChannel(ct);
        await VerifyPreconditions(message, channel, ct);

        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        });
        var body = Encoding.UTF8.GetBytes(json);

        BasicProperties props = new()
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
        };

        var exchange = message.Type;
        await channel.BasicPublishAsync(exchange, string.Empty, mandatory: true,
            basicProperties: props, body: body, cancellationToken: ct);
    }

    private static async Task VerifyPreconditions(OutboxMessage message, IChannel channel, CancellationToken ct)
    {
        var name = message.Type;
        if (!_exchangeStates.TryGetValue(name, out _))
        {
            await DeclareExchange(channel, name, ct);
            _exchangeStates[name] = true;
        }
    }

    private static async Task DeclareExchange(IChannel channel, string name, CancellationToken ct)
    {
        await channel.ExchangeDeclareAsync(name, ExchangeType.Fanout,
            durable: true, autoDelete: false, cancellationToken: ct);
    }
}
