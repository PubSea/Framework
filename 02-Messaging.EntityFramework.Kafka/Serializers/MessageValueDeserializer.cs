using Confluent.Kafka;
using PubSea.Messaging.EntityFramework.Models;
using System.Text;
using System.Text.Json;

namespace PubSea.Messaging.EntityFramework.Kafka.Serializers;

internal sealed class MessageValueDeserializer : IDeserializer<OutboxMessage>
{
    OutboxMessage IDeserializer<OutboxMessage>.Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<OutboxMessage>(json)!;
    }
}
