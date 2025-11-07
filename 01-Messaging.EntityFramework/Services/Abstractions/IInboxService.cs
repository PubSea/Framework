namespace PubSea.Messaging.EntityFramework.Services.Abstractions;

internal interface IInboxService
{
    Task<bool> Exists(long messageId, CancellationToken ct = default);
    Task DoRetentionCycle(CancellationToken ct);
}