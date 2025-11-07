using Confluent.Kafka;
using PubSea.Messaging.EntityFramework.Models;
using System.Text;
using System.Text.Json;

namespace PubSea.Messaging.EntityFramework.Kafka.Serializers;

internal sealed class MessageValueSerializer : IAsyncSerializer<OutboxMessage>
{
    Task<byte[]> IAsyncSerializer<OutboxMessage>.SerializeAsync(OutboxMessage data, SerializationContext context)
    {
        var message = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(message);
        return Task.FromResult(bytes);
    }
}
