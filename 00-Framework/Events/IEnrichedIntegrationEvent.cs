namespace PubSea.Framework.Events;

/// <summary>
/// Interface which every event has to implement it to get recognized as an event. 
/// Type of EventId is long
/// </summary>
public interface IEnrichedIntegrationEvent : IIntegrationEvent
{
    long EventId { get; set; }
    DateTime PublishedUtcDateTime { get; set; }
}

/// <summary>
/// Interface which every event has to implement it to get recognized as an event.
/// </summary>
/// <typeparam name="T">Type of EventId</typeparam>
public interface IEnrichedIntegrationEvent<T> : IIntegrationEvent
{
    T EventId { get; set; }
    DateTime PublishedUtcDateTime { get; set; }
}