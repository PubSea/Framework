using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using PubSea.Framework.Utility;
using System.Data.Common;
using System.Diagnostics;

namespace PubSea.Framework.Data;

/// <summary>
/// Recongnizes heavy queries (queries with more than 500ms processing time) sent to database.
/// </summary>
public sealed class HeavyQueryInterceptor : DbCommandInterceptor
{
    private const int HEAVY_EXECUTION_QUERY_TIME_IN_MILLISECONDS = 500;

    private long _startTime;
    private readonly ILogger<HeavyQueryInterceptor> _logger;

    public HeavyQueryInterceptor(ILogger<HeavyQueryInterceptor> logger)
    {
        _logger = logger;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken ct = default)
    {
        SetStartTime();
        return base.ReaderExecutingAsync(command, eventData, result, ct);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken ct = default)
    {
        Log(command.CommandText);
        return base.ReaderExecutedAsync(command, eventData, result, ct);
    }


    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        SetStartTime();
        return base.ReaderExecuting(command, eventData, result);
    }

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        Log(command.CommandText);
        return base.ReaderExecuted(command, eventData, result);
    }


    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SetStartTime();
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        Log(command.CommandText);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }


    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        SetStartTime();
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        Log(command.CommandText);
        return base.NonQueryExecuted(command, eventData, result);
    }


    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
    {
        SetStartTime();
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = default)
    {
        Log(command.CommandText);
        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }


    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        SetStartTime();
        return base.ScalarExecuting(command, eventData, result);
    }

    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        Log(command.CommandText);
        return base.ScalarExecuted(command, eventData, result);
    }


    private void SetStartTime()
    {
        _startTime = Stopwatch.GetTimestamp();
    }

    private void Log(string command)
    {
        var time = Stopwatch.GetElapsedTime(_startTime);
        if (time < TimeSpan.FromMilliseconds(HEAVY_EXECUTION_QUERY_TIME_IN_MILLISECONDS))
        {
            return;
        }

        _logger.LogHeavyQuery(time, command);
    }
}
