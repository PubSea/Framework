using PubSea.Messaging.EntityFramework.EntityFramework;
using PubSea.Messaging.EntityFramework.Models;
using PubSea.Messaging.EntityFramework.Services.Abstractions;

namespace PubSea.Messaging.EntityFramework.Services.Implementations;

internal sealed class ConsumedFaultService : IConsumedFaultService
{
    private readonly ISeaMessagingDbContext _dbContext;

    public ConsumedFaultService(ISeaMessagingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    async Task IConsumedFaultService.Save(ConsumedFaultMessage faultMessage, CancellationToken ct)
    {
        await _dbContext.ConsumedFaultMessages.AddAsync(faultMessage, ct);
    }
}
