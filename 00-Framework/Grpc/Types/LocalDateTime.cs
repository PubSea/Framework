using ProtoBuf;

namespace PubSea.Framework.Grpc.Types;

[ProtoContract]
public readonly record struct LocalDateTime : IComparable<LocalDateTime>
{
    private readonly DateTime _dateTime;

    [ProtoMember(1)]
    public DateTime DateTime
    {
        get
        {
            return DateTime.SpecifyKind(_dateTime, DateTimeKind.Local);
        }
        init
        {
            _dateTime = value;
        }
    }

    public LocalDateTime()
    { }

    public LocalDateTime(DateTime value)
    {
        DateTime = DateTime.SpecifyKind(value, DateTimeKind.Local);
    }

    public static implicit operator LocalDateTime(DateTime dateTime)
    {
        return new LocalDateTime(dateTime);
    }

    public static implicit operator DateTime(LocalDateTime localDateTime)
    {
        return localDateTime.DateTime;
    }

    int IComparable<LocalDateTime>.CompareTo(LocalDateTime other)
    {
        if (DateTime == other.DateTime)
        {
            return 0;
        }
        else if (DateTime > other.DateTime)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}