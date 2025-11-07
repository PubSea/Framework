namespace PubSea.Messaging.EntityFramework.Kafka.Configs;

/// <summary>
/// Configuration of Kafka setup.
/// </summary>
public sealed class KafkaConfig
{
    /// <summary>
    /// The Kafka ClientId.
    /// </summary>
    public string ClientId { get; set; } = null!;

    /// <summary>
    /// Full connection string in order to connect to Kafka.
    /// </summary>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// Topic name for outbox messages
    /// </summary>
    public string TopicName { get; set; } = null!;

    /// <summary>
    /// List of consuming topic names
    /// </summary>
    public List<string> ConsumingTopicNames { get; set; } = [];

    /// <summary>
    /// Count of partitions for outbox topic. Default is 10
    /// </summary>
    public byte Partitions { get; set; } = 10;

    /// <summary>
    /// Group id in order to recognize group and assign partitions in distributed applictions.
    /// </summary>
    public string ConsumerGroupId { get; set; } = null!;

    /// <summary>
    /// Number of concurrent consumers consumes messages of different partitions.
    /// </summary>
    public byte ConcurrentConsumers { get; set; } = 1;
}
