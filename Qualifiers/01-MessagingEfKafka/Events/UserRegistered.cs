using PubSea.Framework.Events;
using Sea.MessagingContracts.UsersManagement.User;

namespace MessagingEfKafka.Events;

public sealed class UserRegistered : IUserRegistered, IIntegrationEvent
{
    public long Id { get; init; }
    public string UserName { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
}
