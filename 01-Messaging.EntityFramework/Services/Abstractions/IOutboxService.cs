using PubSea.Framework.Events;
using PubSea.Messaging.EntityFramework.Models;
using System.Collections.Immutable;

namespace PubSea.Messaging.EntityFramework.Services.Abstractions;

internal interface IOutboxService
{
    IImmutableList<OutboxMessage> CreateMessage(params IEnumerable<IEvent> events);
    IImmutableList<OutboxMessage> CreateMessage(string? prioritizerKey, params IEnumerable<IEvent> events);
    Task<ICollection<OutboxMessage>> GetUnpublishedMessages(CancellationToken ct = default);
    Task UpdatePublishedMessages(ICollection<OutboxMessage> messages, CancellationToken ct = default);
    Task RemovePublishedMessages(ICollection<OutboxMessage> messages, CancellationToken ct = default);
}
