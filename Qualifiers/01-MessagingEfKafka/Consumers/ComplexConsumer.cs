using MessagingEfKafka.Events;
using PubSea.Messaging.EntityFramework.Services.Abstractions;

namespace MessagingEfKafka.Consumers;

internal sealed class ComplexConsumer : ISeaConsumer<IComplexMessage>
{
    async Task ISeaConsumer<IComplexMessage>.Consume(IComplexMessage messagePayload, CancellationToken ct)
    {
        Console.WriteLine("********** Complex message consumed **********");
        await Task.CompletedTask;
    }
}
