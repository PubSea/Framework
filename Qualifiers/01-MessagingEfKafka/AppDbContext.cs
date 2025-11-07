using Microsoft.EntityFrameworkCore;
using PubSea.Messaging.EntityFramework.EntityFramework;
using PubSea.Messaging.EntityFramework.Models;

namespace MessagingEfKafka;

public class AppDbContext : DbContext, ISeaMessagingDbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Profile> Profiles { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }
    public DbSet<ConsumedFaultMessage> ConsumedFaultMessages { get; set; }
}
