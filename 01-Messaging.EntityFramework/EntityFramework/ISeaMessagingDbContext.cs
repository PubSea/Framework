using Microsoft.EntityFrameworkCore;
using PubSea.Messaging.EntityFramework.Models;

namespace PubSea.Messaging.EntityFramework.EntityFramework;

/// <summary>
/// This interface MUST be implemented by application dbcontext so messaging system can work
/// </summary>
public interface ISeaMessagingDbContext
{
    /// <summary>
    /// This property defines a table for outbox messages.
    /// </summary>
    /// <remarks>
    /// All table configs are configurable by implementing <see cref="IEntityTypeConfiguration{TEntity}"/> in
    /// application using EF fluent api and reference it in <see cref="DbContext.OnModelCreating(ModelBuilder)"/>
    /// </remarks>
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    /// <summary>
    /// This property defines a table for inbox messages.
    /// </summary>
    /// <remarks>
    /// All table configs are configurable by implementing <see cref="IEntityTypeConfiguration{TEntity}"/> in
    /// application using EF fluent api and reference it in <see cref="DbContext.OnModelCreating(ModelBuilder)"/>
    /// </remarks>
    public DbSet<InboxMessage> InboxMessages { get; set; }

    /// <summary>
    /// This property defines a table for those messages that face exception during process.
    /// </summary>
    /// <remarks>
    /// All table configs are configurable by implementing <see cref="IEntityTypeConfiguration{TEntity}"/> in
    /// application using EF fluent api and reference it in <see cref="DbContext.OnModelCreating(ModelBuilder)"/>
    /// </remarks>
    public DbSet<ConsumedFaultMessage> ConsumedFaultMessages { get; set; }
}
