using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PubSea.Messaging.EntityFramework.Configs;
using PubSea.Messaging.EntityFramework.Services.Abstractions;
using System.Diagnostics;

namespace PubSea.Messaging.EntityFramework.HostedServices;

internal sealed class OutboxPollingPublisher : BackgroundService
{
    private readonly SeaMessagingConfig _config;
    private readonly ILogger<OutboxPollingPublisher> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OutboxPollingPublisher(SeaMessagingConfig config, ILogger<OutboxPollingPublisher> logger,
        IServiceProvider serviceProvider)
    {
        _config = config;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxPollingPublisher started working {StartDate}", DateTime.UtcNow);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var startTime = Stopwatch.GetTimestamp();

                using var scope = _serviceProvider.CreateScope();
                var outbox = scope.ServiceProvider.GetRequiredService<IOutboxService>();
                var publisher = scope.ServiceProvider.GetRequiredService<IBrokerService>();

                var messages = await outbox.GetUnpublishedMessages(stoppingToken);
                if (messages.Count == 0)
                {
                    await RemoveEarlyPublishedMessagesIfNecessary(outbox, stoppingToken);

                    await Delay(startTime, _config.OutboxPollingInterval, stoppingToken);
                    continue;
                }

                await publisher.TransportMessages(messages, stoppingToken);

                await outbox.RemovePublishedMessages(messages, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong in OutboxPollingPublisher");
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogCritical("OutboxPollingPublisher stopped working {StopDate}", DateTime.UtcNow);
        return base.StopAsync(cancellationToken);
    }

    private async Task RemoveEarlyPublishedMessagesIfNecessary(IOutboxService outbox, CancellationToken stoppingToken)
    {
        if (!_config.PublishOutboxInstantly)
        {
            return;
        }

        await outbox.RemovePublishedMessages([], stoppingToken);
    }

    private static async Task Delay(long startTime, TimeSpan minDelay, CancellationToken ct)
    {
        var remainingTimeToDelay = minDelay - Stopwatch.GetElapsedTime(startTime);
        if (remainingTimeToDelay > TimeSpan.Zero)
        {
            await Task.Delay(remainingTimeToDelay, ct);
        }
    }
}