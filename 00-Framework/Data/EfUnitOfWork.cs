using Microsoft.EntityFrameworkCore;
using System.Data;

namespace PubSea.Framework.Data;

internal sealed class EfUnitOfWork<T> : IEfUnitOfWork
    where T : DbContext
{
    private readonly T _dbContext;

    public EfUnitOfWork(T dbContext)
    {
        _dbContext = dbContext;
    }

    async Task IUnitOfWork.BeginTransaction(IsolationLevel isolationLevel, CancellationToken ct)
    {
        await _dbContext.Database.BeginTransactionAsync(isolationLevel, ct);
    }

    async Task IUnitOfWork.CommitTransaction(CancellationToken ct)
    {
        await _dbContext.Database.CommitTransactionAsync(ct);

        if (_dbContext.Database.CurrentTransaction is not null)
        {
            await _dbContext.Database.CurrentTransaction.DisposeAsync();
        }
    }

    async Task IUnitOfWork.RollbackTransaction(CancellationToken ct)
    {
        await _dbContext.Database.RollbackTransactionAsync(ct);

        if (_dbContext.Database.CurrentTransaction is not null)
        {
            await _dbContext.Database.CurrentTransaction.DisposeAsync();
        }
    }

    async Task IUnitOfWork.SaveChanges(CancellationToken ct)
    {
        await _dbContext.SaveChangesAsync(ct);
    }

    async Task IEfUnitOfWork.WithExecutionStrategy(Func<CancellationToken, Task> func, CancellationToken ct)
    {
        await _dbContext.Database.CreateExecutionStrategy().ExecuteAsync(func, ct);
    }

    async Task<R> IEfUnitOfWork.WithExecutionStrategy<R>(Func<CancellationToken, Task<R>> func, CancellationToken ct)
    {
        return await _dbContext.Database.CreateExecutionStrategy().ExecuteAsync(func, ct);
    }
}
