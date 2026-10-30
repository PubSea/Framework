using MessagingEfKafka.Events;
using PubSea.Messaging.EntityFramework.Services.Abstractions;

namespace MessagingEfKafka.Consumers;

internal sealed class ProfileCreatedConsumer : ISeaConsumer<IUserRegistered>
{
    async Task ISeaConsumer<IUserRegistered>.Consume(IUserRegistered messagePayload, CancellationToken ct)
    {
        Console.WriteLine("********** Profile Created **********");
        await Task.CompletedTask;
    }
}
