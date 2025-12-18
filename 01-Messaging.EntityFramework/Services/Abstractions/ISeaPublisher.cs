using PubSea.Framework.Events;

namespace PubSea.Messaging.EntityFramework.Services.Abstractions;

/// <summary>
/// This is an atomic publisher which can save all messages and the main data simultaneously.
/// </summary>
public interface ISeaPublisher
{
    /// <summary>
    /// Publishes messages regardless prioritization. If order of processing matters use 
    /// <see cref="Publish(IReadOnlyList{IEvent}, string, CancellationToken)"/> should be used
    /// </summary>
    /// <param name="events"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task Publish(IReadOnlyList<IEvent> events, CancellationToken ct = default);

    /// <summary>
    /// Publishes messages considering prioritization. If order of processing does not matter use
    /// <see cref="Publish(IReadOnlyList{IEvent}, CancellationToken)"/> should be used
    /// </summary>
    /// <param name="events"></param>
    /// <param name="prioritizerKey"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task Publish(IReadOnlyList<IEvent> events, string prioritizerKey, CancellationToken ct = default);

    /// <summary>
    /// Publishes messages regardless prioritization directly to broker without saving them first in database. 
    /// If order of processing matters use 
    /// <see cref="PublishDirectly(IReadOnlyList{IEvent}, string, CancellationToken)"/> should be used
    /// </summary>
    /// <param name="events"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task PublishDirectly(IReadOnlyList<IEvent> events, CancellationToken ct = default);

    /// <summary>
    /// Publishes messages considering prioritization directly to broker without saving them first in database. 
    /// If order of processing does not matter use
    /// <see cref="PublishDirectly(IReadOnlyList{IEvent}, CancellationToken)"/> should be used
    /// </summary>
    /// <param name="events"></param>
    /// <param name="prioritizerKey"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task PublishDirectly(IReadOnlyList<IEvent> events, string prioritizerKey, CancellationToken ct = default);
}
