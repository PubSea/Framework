using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PubSea.Messaging.EntityFramework.EntityFramework;
using PubSea.Messaging.EntityFramework.Models;
using PubSea.Messaging.EntityFramework.Services.Abstractions;

namespace PubSea.Messaging.EntityFramework.HostedServices;

internal sealed class OutboxMessageTransporter : BackgroundService
{
    private readonly ILogger<OutboxMessageTransporter> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OutboxMessageTransporter(ILogger<OutboxMessageTransporter> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxMessageTransporter started working {StartDate}", DateTime.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var broker = scope.ServiceProvider.GetRequiredService<IBrokerService>();

        await foreach (var messages in OutboxChannel.Instance.Reader.ReadAllAsync(stoppingToken))
        {
            _ = Task.Factory.StartNew(async () =>
            {
                try
                {
                    await broker.TransportMessages(messages, stoppingToken);
                    await UpdateOutboxMessages(messages, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Something went wrong in OutboxMessageTransporter");
                }
            }, TaskCreationOptions.LongRunning);
        }

        async Task UpdateOutboxMessages(ICollection<OutboxMessage> messages, CancellationToken stoppingToken)
        {
            using var dbScope = _serviceProvider.CreateScope();
            var dbContext = dbScope.ServiceProvider.GetRequiredService<ISeaMessagingDbContext>();
            foreach (var message in messages)
            {
                await dbContext.OutboxMessages
                    .Where(m => m.Id == message.Id)
                    .ExecuteUpdateAsync(m => m.SetProperty(d => d.IsPublished, true), stoppingToken);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogCritical("OutboxMessageTransporter stopped working {StopDate}", DateTime.UtcNow);
        return base.StopAsync(cancellationToken);
    }
}
