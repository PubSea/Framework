using Microsoft.Extensions.DependencyInjection;

namespace PubSea.Messaging.EntityFramework.Configs;

public sealed class SeaMessagingConfig
{
    internal IServiceCollection Services { get; set; } = null!;
    internal BrokerType BrokerType { get; set; }

    /// <summary>
    /// <para>
    /// Specifies if messages publish instantly after adding to database or not. 
    /// Notice if this is true, where ever a flow or prioritization exists, might face problems 
    /// because it is possible transporting message to broker have error and the flow breaks. 
    /// </para>
    /// <para>
    /// If a flow
    /// or prioritization exists it is better this propery be false and have short <see cref="OutboxPollingInterval"/> but
    /// if this conditions does not exist it is more performant to have this equal to true
    /// with long <see cref="OutboxPollingInterval"/>
    /// </para>
    /// </summary>
    public bool PublishOutboxInstantly { get; set; } = false;

    /// <summary>
    /// <para>
    /// The period of time for each cycle in order to detect and publish unpublished messages 
    /// to broker then remove them from database.
    /// </para> 
    /// <para>If <see cref="PublishOutboxInstantly"/> is true it is better 
    /// to have this time longer than against when <see cref="PublishOutboxInstantly"/> is false
    /// </para>
    /// </summary>
    public TimeSpan OutboxPollingInterval { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// The period to do retention cycle on inbox messages and remove old ones.
    /// </summary>
    public TimeSpan InboxRetentionInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// The time to live (expiry) for inbox messages.
    /// </summary>
    public TimeSpan InboxMessageTtl { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// This provider only logs messages to console and does NOT do anything more. It is good
    /// for debug purposes.
    /// </summary>
    public void UseDefaultBroker()
    {
        BrokerType = BrokerType.None;
    }
}
