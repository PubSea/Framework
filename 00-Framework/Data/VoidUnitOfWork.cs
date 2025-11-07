using System.Data;

namespace PubSea.Framework.Data;

/// <summary>
/// This is just a void unit of work. It does nothing special and is useful in mocking or 
/// for non transactional database engines
/// </summary>
public sealed class VoidUnitOfWork : IUnitOfWork
{
    public async Task BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public async Task RollbackTransaction(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public async Task CommitTransaction(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }
}
