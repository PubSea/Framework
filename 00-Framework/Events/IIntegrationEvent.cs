namespace PubSea.Framework.Events;

/// <summary>
/// Interface which every event has to implement it to get recognized as an integration event. 
/// Type of EventId is long
/// </summary>
public interface IIntegrationEvent : IEvent
{ }
