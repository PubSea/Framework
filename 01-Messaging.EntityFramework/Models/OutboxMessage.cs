using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PubSea.Messaging.EntityFramework.Models;

/// <summary>
/// Outbox message declaration
/// </summary>
[Table("outbox_messages", Schema = "messaging")]
[PrimaryKey(nameof(UtcCreationDate), nameof(Id))]
public sealed class OutboxMessage
{
    /// <summary>
    /// Original creation date of message.
    /// </summary>
    public DateTime UtcCreationDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// key of outbox message. This key is exactly equal to <see cref="InboxMessage.Id"/> or
    /// <see cref="ConsumedFaultMessage.Id"/>.
    /// </summary>
    public long Id { get; init; }

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

    [JsonIgnore]
    public bool IsPublished { get; private set; }
}
