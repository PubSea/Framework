using PubSea.Framework.Events;

namespace MessagingEfKafka.Events;

public sealed class UserRegistered : IUserRegistered, IIntegrationEvent
{
    public long Id { get; init; }
    public string UserName { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
}