using System.Threading.Channels;

namespace PubSea.Messaging.EntityFramework.Models;

internal static class OutboxChannel
{
    internal static readonly Channel<ICollection<OutboxMessage>> Instance =
        Channel.CreateUnbounded<ICollection<OutboxMessage>>(new UnboundedChannelOptions
        {
            SingleWriter = false,
            SingleReader = false,
            AllowSynchronousContinuations = true,
        });
}
