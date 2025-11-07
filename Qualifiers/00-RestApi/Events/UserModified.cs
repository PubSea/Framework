using PubSea.Framework.Events;

namespace RestApi.Events;

public sealed class UserModified : IEnrichedDomainEvent
{
    public long EventId { get; set; }
    public DateTime PublishedUtcDateTime { get; set; }

    public int UserId { get; set; } = 10;
}