namespace MessagingEfKafka.Events;

public interface IUserRegistered
{
    long Id { get; init; }
    string UserName { get; init; }
    string PhoneNumber { get; init; }
}