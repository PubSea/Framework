using PubSea.Messaging.EntityFramework.Models;

namespace PubSea.Messaging.EntityFramework.Services.Abstractions;

internal interface IBrokerService
{
    Task TransportMessages(ICollection<OutboxMessage> messages, CancellationToken ct = default);
}
