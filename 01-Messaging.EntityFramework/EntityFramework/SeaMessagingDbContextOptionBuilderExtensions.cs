using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PubSea.Messaging.EntityFramework.Configs;
using PubSea.Messaging.EntityFramework.Interceptors;
using PubSea.Messaging.EntityFramework.Services.Abstractions;

namespace PubSea.Messaging.EntityFramework.EntityFramework;

internal static class SeaMessagingDbContextOptionBuilderExtensions
{
    internal static DbContextOptionsBuilder UseSeaMessaging(this DbContextOptionsBuilder options, IServiceProvider provider)
    {
        var messagingLogger = provider.GetRequiredService<ILogger<OutboxSavedChangesInterceptor>>();
        var outboxPublisher = provider.GetRequiredService<IBrokerService>();
        var config = provider.GetRequiredService<SeaMessagingConfig>();
        options.AddInterceptors(new OutboxSavedChangesInterceptor(config, messagingLogger, outboxPublisher));

        return options;
    }
}