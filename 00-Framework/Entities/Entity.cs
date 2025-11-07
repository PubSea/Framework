using PubSea.Framework.Events;

namespace PubSea.Framework.Entities;

/// <summary>
/// Abstract class to be inherited by entities.
/// </summary>
/// <typeparam name="TId">Type of entity Id</typeparam>
public abstract class Entity<TId> where TId : IEquatable<TId>
{
    protected readonly List<IEvent> _events = [];

    /// <summary>
    /// Type of Id
    /// </summary>
    public TId? Id { get; protected set; }

    /// <summary>
    /// Identifies state if specified event occurs.
    /// </summary>
    /// <param name="event"></param>
    protected abstract void SetStateByEvent(IEvent @event);

    /// <summary>
    /// Validates invariants of the object.
    /// </summary>
    protected abstract void ValidateInvariants();

    /// <summary>
    /// Method to handle event.
    /// </summary>
    /// <param name="event">event</param>
    protected void Emit(IEvent @event)
    {
        SetStateByEvent(@event);
        ValidateInvariants();
        _events.Add(@event);
    }

    /// <summary>
    /// Returns all object events.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<IEvent> GetEvents()
    {
        return _events.AsReadOnly();
    }

    /// <summary>
    /// Clears all events.
    /// </summary>
    public void ClearEvents()
    {
        _events.Clear();
    }

    /// <summary>
    /// Checks equality of object with another object.
    /// </summary>
    /// <param name="obj">other object</param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Entity<TId> other ||
            GetType() != other.GetType() ||
            Id is null ||
            other.Id is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.GetHashCode() == other.Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Returns object hash code.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var id = Id?.GetHashCode().ToString() ?? Guid.NewGuid().ToString();
        return (GetType().ToString() + id).GetHashCode();
    }
}