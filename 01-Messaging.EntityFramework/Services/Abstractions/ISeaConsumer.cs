namespace PubSea.Messaging.EntityFramework.Services.Abstractions;

/// <summary>
/// The main interface that all consumers MUST implement in order to consume receiving message.
/// </summary>
/// <remarks>
/// NOTICE for every message type ONLY one implementation is needed and other implementations 
/// does not get called
/// </remarks>
/// <typeparam name="TMessage">Type of message</typeparam>
public interface ISeaConsumer<TMessage>
{
    Task Consume(TMessage messagePayload, CancellationToken ct = default);
}
