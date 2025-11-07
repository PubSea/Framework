using Microsoft.EntityFrameworkCore;
using PubSea.Messaging.EntityFramework.Configs;
using PubSea.Messaging.EntityFramework.EntityFramework;
using PubSea.Messaging.EntityFramework.Services.Abstractions;

namespace PubSea.Messaging.EntityFramework.Services.Implementations;

internal sealed class InboxService : IInboxService
{
    private readonly SeaMessagingConfig _config;
    private readonly ISeaMessagingDbContext _dbContext;

    public InboxService(SeaMessagingConfig config, ISeaMessagingDbContext dbContext)
    {
        _config = config;
        _dbContext = dbContext;
    }

    async Task<bool> IInboxService.Exists(long messageId, CancellationToken ct)
    {
        return await _dbContext.InboxMessages.AnyAsync(m => m.Id == messageId, ct);
    }

    async Task IInboxService.DoRetentionCycle(CancellationToken ct)
    {
        var retentionDate = DateTime.UtcNow.Subtract(_config.InboxMessageTtl);

        await _dbContext.InboxMessages
            .Where(m => m.UtcReceivedDate <= retentionDate)
            .ExecuteDeleteAsync(ct);
    }
}
