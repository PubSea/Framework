using PubSea.Framework.Events;
using PubSea.Framework.Exceptions;
using PubSea.Messaging.EntityFramework.EntityFramework;
using PubSea.Messaging.EntityFramework.Services.Abstractions;

namespace PubSea.Messaging.EntityFramework.Services.Implementations;

internal sealed class SeaPublisher : ISeaPublisher
{
    private readonly IOutboxService _outboxService;
    private readonly ISeaMessagingDbContext _dbContext;
    private readonly IBrokerService _brokerService;

    public SeaPublisher(IOutboxService outboxService, ISeaMessagingDbContext dbContext,
        IBrokerService brokerService)
    {
        _outboxService = outboxService;
        _dbContext = dbContext;
        _brokerService = brokerService;
    }

    async Task ISeaPublisher.Publish(IReadOnlyList<IEvent> events, CancellationToken ct)
    {
        var messages = _outboxService.CreateMessage(events);
        await _dbContext.OutboxMessages.AddRangeAsync(messages, ct);
    }

    async Task ISeaPublisher.Publish(IReadOnlyList<IEvent> events, string prioritizerKey, CancellationToken ct)
    {
        VerifyPrioritizerKey(prioritizerKey);

        var messages = _outboxService.CreateMessage(prioritizerKey, events);
        await _dbContext.OutboxMessages.AddRangeAsync(messages, ct);
    }

    async Task ISeaPublisher.PublishDirectly(IReadOnlyList<IEvent> events, CancellationToken ct)
    {
        var messages = _outboxService.CreateMessage(events);
        await _brokerService.TransportMessages([.. messages], ct);
    }

    async Task ISeaPublisher.PublishDirectly(IReadOnlyList<IEvent> events, string prioritizerKey, CancellationToken ct)
    {
        VerifyPrioritizerKey(prioritizerKey);
        var messages = _outboxService.CreateMessage(prioritizerKey, events);
        await _brokerService.TransportMessages([.. messages], ct);
    }

    private static void VerifyPrioritizerKey(string prioritizerKey)
    {
        if (string.IsNullOrWhiteSpace(prioritizerKey))
        {
            throw new SeaException("Prioritizer key can not be null",
                SeaException.INTERNAL_ERROR_CODE, ExceptionStatus.Internal);
        }
    }
}
