using MessagingEfKafka.Events;
using PubSea.Framework.Entities;
using PubSea.Framework.Events;

namespace MessagingEfKafka;

public sealed class Profile : AggregateRoot<long>
{
    public static Profile Create(long id, string firstName, string lastName)
    {
        Profile user = new();

        user.Emit(new UserRegistered
        {
            Id = id,
            UserName = firstName,
            PhoneNumber = lastName,
        });

        return user;
    }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    protected override void SetStateByEvent(IEvent @event)
    {
        switch (@event)
        {
            case UserRegistered e:
                Id = e.Id;
                FirstName = e.UserName;
                LastName = e.PhoneNumber;
                break;
            default:
                break;
        }
    }

    protected override void ValidateInvariants()
    {
        return;
    }

}
