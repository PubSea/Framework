using Microsoft.EntityFrameworkCore;
using PubSea.Framework.Events;
using PubSea.Framework.Exceptions;
using PubSea.Messaging.EntityFramework.EntityFramework;
using PubSea.Messaging.EntityFramework.Models;
using PubSea.Messaging.EntityFramework.Services.Abstractions;
using PubSea.Framework.Services.Abstractions;
using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PubSea.Messaging.EntityFramework.Services.Implementations;

internal sealed class OutboxService : IOutboxService
{
    private readonly ISnowflakeService _snowflakeService;
    private readonly ISeaMessagingDbContext _dbContext;

    public OutboxService(ISnowflakeService snowflakeService, ISeaMessagingDbContext dbContext)
    {
        _snowflakeService = snowflakeService;
        _dbContext = dbContext;
    }

    IImmutableList<OutboxMessage> IOutboxService.CreateMessage(params IEnumerable<IEvent> events)
    {
        return ((IOutboxService)this).CreateMessage(prioritizerKey: null, events);
    }

    IImmutableList<OutboxMessage> IOutboxService.CreateMessage(string? prioritizerKey, params IEnumerable<IEvent> events)
    {
        var messages = new List<OutboxMessage>(events.Count());
        foreach (object @event in events)
        {
            var messageId = _snowflakeService.CreateId();

            var payload = JsonSerializer.Serialize(@event, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            });

            var realTypeName = @event.GetType().Name;
            var type = @event.GetType().GetInterface($"I{realTypeName}")?.FullName ??
                throw new SeaException(
                    $$"""
                    All events MUST implement an interface with their same name starting with a capital 'I' following by event name 
                    like (EventHappened : IEventHappened).
                    {{realTypeName}} does not implement an interface naming I{{realTypeName}}. 
                    """,
                    SeaException.INTERNAL_ERROR_CODE, ExceptionStatus.Internal);

            var message = new OutboxMessage
            {
                Id = messageId,
                Payload = payload,
                Type = type,
                PrioritizerKey = prioritizerKey,
            };

            messages.Add(message);
        }

        return messages.ToImmutableList();
    }

    async Task<ICollection<OutboxMessage>> IOutboxService.GetUnpublishedMessages(CancellationToken ct)
    {
        var limitPerRead = 100;
        return await _dbContext.OutboxMessages
            .Where(m => !m.IsPublished)
            .OrderBy(m => m.UtcCreationDate)
            .Take(limitPerRead)
            .ToListAsync(ct);
    }

    async Task IOutboxService.UpdatePublishedMessages(ICollection<OutboxMessage> messages, CancellationToken ct)
    {
        await _dbContext.OutboxMessages
            .Where(m => messages.Select(m => m.Id).Contains(m.Id))
            .ExecuteUpdateAsync(m => m.SetProperty(d => d.IsPublished, true), ct);
    }


    async Task IOutboxService.RemovePublishedMessages(ICollection<OutboxMessage> messages, CancellationToken ct)
    {
        if (messages.Count == 0)
        {
            await _dbContext.OutboxMessages
                .Where(om => om.IsPublished)
                .ExecuteDeleteAsync(ct);
        }
        else
        {
            await _dbContext.OutboxMessages
                .Where(om => messages.Select(m => m.Id).Contains(om.Id) || om.IsPublished)
                .ExecuteDeleteAsync(ct);
        }
    }
}
