using Confluent.Kafka;
using System.Text;

namespace PubSea.Messaging.EntityFramework.Kafka.Serializers;

internal sealed class MessageKeyDeserializer : IDeserializer<long>
{
    long IDeserializer<long>.Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return long.Parse(Encoding.UTF8.GetString(data));
    }
}
