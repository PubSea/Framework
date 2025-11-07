using PubSea.Framework.Events;

namespace RestApi.Events;

public sealed class UserCreated : IEnrichedDomainEvent
{
    public long EventId { get; set; }
    public DateTime PublishedUtcDateTime { get; set; }
}
