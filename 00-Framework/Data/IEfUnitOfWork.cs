namespace PubSea.Framework.Data;

/// <summary>
/// Entity framwork unit of work including execution strategy methods.
/// </summary>
public interface IEfUnitOfWork : IUnitOfWork
{
    Task WithExecutionStrategy(Func<CancellationToken, Task> func, CancellationToken ct = default);

    Task<T> WithExecutionStrategy<T>(Func<CancellationToken, Task<T>> func, CancellationToken ct = default);
}