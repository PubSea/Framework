using Confluent.Kafka;
using System.Text;

namespace PubSea.Messaging.EntityFramework.Kafka.Serializers;

internal sealed class MessageKeySerializer : IAsyncSerializer<long>
{
    Task<byte[]> IAsyncSerializer<long>.SerializeAsync(long data, SerializationContext context)
    {
        var bytes = Encoding.UTF8.GetBytes(data.ToString());
        return Task.FromResult(bytes);
    }
}
