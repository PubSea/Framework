namespace PubSea.Framework.Events;

/// <summary>
/// Interface which every internal event has to implement it to get recognized as an internal event. 
/// Type of EventId is long
/// </summary>
public interface IEnrichedDomainEvent : IDomainEvent
{
    long EventId { get; set; }
    DateTime PublishedUtcDateTime { get; set; }
}

/// <summary>
/// Interface which every internal event has to implement it to get recognized as an internal event. 
/// </summary>
/// <typeparam name="T">Type of EventId</typeparam>
public interface IEnrichedDomainEvent<T> : IDomainEvent
{
    T EventId { get; set; }
    DateTime PublishedUtcDateTime { get; set; }
}