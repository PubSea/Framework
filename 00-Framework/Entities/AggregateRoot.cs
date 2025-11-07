namespace PubSea.Framework.Entities;

/// <summary>
/// Abstract class to be inherited by aggregate roots.
/// </summary>
/// <typeparam name="TId">Type of aggregate root Id</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId> where TId : IEquatable<TId>
{ }