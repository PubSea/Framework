using RabbitMQ.Client;

namespace PubSea.Messaging.EntityFramework.RabbitMq.Services.Abstractions;

internal interface IRabbitMqClient
{
    Task<IChannel> GetChannel(CancellationToken ct = default);
    Task<IChannel> GetNewChannel(CancellationToken ct = default);
}
