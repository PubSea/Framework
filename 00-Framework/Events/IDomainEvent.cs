namespace PubSea.Framework.Events;

/// <summary>
/// Interface which every event has to implement it to get recognized as a domain event. 
/// Type of EventId is long
/// </summary>
public interface IDomainEvent : IEvent
{ }
