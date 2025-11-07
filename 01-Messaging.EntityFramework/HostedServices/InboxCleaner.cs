using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PubSea.Messaging.EntityFramework.Configs;
using PubSea.Messaging.EntityFramework.Services.Abstractions;
using System.Diagnostics;

namespace PubSea.Messaging.EntityFramework.HostedServices;

internal sealed class InboxCleaner : BackgroundService
{
    private readonly SeaMessagingConfig _config;
    private readonly ILogger<InboxCleaner> _logger;
    private readonly IServiceProvider _serviceProvider;

    public InboxCleaner(SeaMessagingConfig config, ILogger<InboxCleaner> logger,
        IServiceProvider serviceProvider)
    {
        _config = config;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("InboxCleaner started working {StartDate}", DateTime.UtcNow);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var startTime = Stopwatch.GetTimestamp();

                using var scope = _serviceProvider.CreateScope();
                var inbox = scope.ServiceProvider.GetRequiredService<IInboxService>();
                await inbox.DoRetentionCycle(stoppingToken);

                await Delay(startTime, _config.InboxRetentionInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong in InboxCleaner");
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogCritical("InboxCleaner stopped working {StopDate}", DateTime.UtcNow);
        return base.StopAsync(cancellationToken);
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