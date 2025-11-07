namespace PubSea.Messaging.EntityFramework.RabbitMq.Configs;

/// <summary>
/// Configuration of RabbitMq setup.
/// </summary>
public sealed class RabbitMqConfig
{
    /// <summary>
    /// The RabbitMq ClientId.
    /// </summary>
    public string ClientId { get; set; } = null!;

    /// <summary>
    /// Full connection string in order to connect to RabbitMq.
    /// </summary>
    public string ConnectionString { get; set; } = null!;
}
