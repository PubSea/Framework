using PubSea.Messaging.EntityFramework.Models;

namespace PubSea.Messaging.EntityFramework.Services.Abstractions;

internal interface IConsumedFaultService
{
    Task Save(ConsumedFaultMessage faultMessage, CancellationToken ct = default);
}