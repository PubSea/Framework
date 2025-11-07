using Microsoft.Extensions.DependencyInjection;
using PubSea.Messaging.EntityFramework.Models;

namespace PubSea.Messaging.EntityFramework.Services.Abstractions;

internal interface IMainConsumer
{
    Task Consume(IServiceScope scope, OutboxMessage message, CancellationToken ct = default);
}
