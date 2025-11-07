using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PubSea.Messaging.EntityFramework.Models;

/// <summary>
/// Fault message declaration
/// </summary>
[Table("consumed_fault_messages", Schema = "messaging")]
public sealed class ConsumedFaultMessage
{
    /// <summary>
    /// key of consumed message. This key is exactly equal to <see cref="OutboxMessage.Id"/> or
    /// <see cref="InboxMessage.Id"/>
    /// </summary>
    [Key]
    public long Id { get; init; }

    /// <summary>
    /// Original creation date of message. This key is exactly equal to <see cref="OutboxMessage.UtcCreationDate"/>.
    /// </summary>
    public DateTime UtcCreationDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// A json containing transported data.
    /// </summary>
    public string Payload { get; init; } = null!;

    /// <summary>
    /// The clr type of original message.
    /// </summary>
    [StringLength(maximumLength: 250)]
    public string Type { get; init; } = null!;

    /// <summary>
    /// The key in order to keep prioritization of message processing.
    /// </summary>
    [StringLength(maximumLength: 100)]
    public string? PrioritizerKey { get; init; } = null!;

    /// <summary>
    /// The exception info occured during processing received message
    /// </summary>
    public string Exception { get; init; } = null!;
}
