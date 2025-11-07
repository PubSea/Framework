using System.Data;

namespace PubSea.Framework.Data;

/// <summary>
/// Unit of work in order to encapsulate a transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Starts a new transaction asynchronously.
    /// </summary>
    /// <returns></returns>
    Task BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken ct = default);

    /// <summary>
    /// Rollbacks manual transaction asynchronously.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task RollbackTransaction(CancellationToken ct = default);

    /// <summary>
    /// Commits manual transaction asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation</returns>
    Task CommitTransaction(CancellationToken ct = default);

    /// <summary>
    /// Commits internal transaction asynchronously.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns>A task that represents the asynchronous save operation</returns>
    Task SaveChanges(CancellationToken ct = default);
}
