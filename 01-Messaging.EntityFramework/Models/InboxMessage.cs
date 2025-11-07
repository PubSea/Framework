using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PubSea.Messaging.EntityFramework.Models;

/// <summary>
/// Inbox message declaration
/// </summary>
[Table("inbox_messages", Schema = "messaging")]
public sealed class InboxMessage
{
    /// <summary>
    /// key of inbox message. This key is exactly equal to <see cref="OutboxMessage.Id"/> or
    /// <see cref="ConsumedFaultMessage.Id"/>.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// The time message received.
    /// </summary>
    public DateTime UtcReceivedDate { get; init; } = DateTime.UtcNow;
}