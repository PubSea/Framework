namespace PubSea.Framework.Events;

/// <summary>
/// Interface that should be implemented by every dispatcher class.
/// </summary>
public interface IEventDispatcher
{
    Task Dispatch<TEvent>(TEvent[] events, CancellationToken ct = default) where TEvent : IDomainEvent;
}