using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using PubSea.Messaging.EntityFramework.Configs;
using PubSea.Messaging.EntityFramework.Models;
using PubSea.Messaging.EntityFramework.Services.Abstractions;

namespace PubSea.Messaging.EntityFramework.Interceptors;

internal sealed class OutboxSavedChangesInterceptor : SaveChangesInterceptor
{
    private readonly SeaMessagingConfig _config;
    private readonly ILogger<OutboxSavedChangesInterceptor> _logger;
    private readonly IBrokerService _outboxPublisher;

    internal OutboxSavedChangesInterceptor(SeaMessagingConfig config, ILogger<OutboxSavedChangesInterceptor> logger,
        IBrokerService outboxPublisher)
    {
        _config = config;
        _logger = logger;
        _outboxPublisher = outboxPublisher;
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken ct = default)
    {
        await PrepareMessagesFroTransferringToBroker(eventData, ct);

        return await base.SavedChangesAsync(eventData, result, ct);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        _ = PrepareMessagesFroTransferringToBroker(eventData);

        return base.SavedChanges(eventData, result);
    }

    private async ValueTask PrepareMessagesFroTransferringToBroker(SaveChangesCompletedEventData eventData, CancellationToken ct = default)
    {
        try
        {
            if (!_config.PublishOutboxInstantly || eventData.Context is null)
            {
                return;
            }

            var messages = eventData.Context.ChangeTracker
                .Entries<OutboxMessage>()
                .Select(e => e.Entity)
                .Where(m => !m.IsPublished)
                .ToList();

            if (messages.Count == 0)
            {
                return;
            }

            await OutboxChannel.Instance.Writer.WriteAsync(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Something went wrong in publishing messages to broker after savechanges completed");
        }
    }
}
