using PubSea.Framework.Services.Abstractions;
using StackExchange.Redis;

namespace PubSea.Framework.Services.Implementations;

internal sealed class SeaConnectionMultiplexer : ISeaConnectionMultiplexer
{
    private readonly Lock _locker = new();
    private static IConnectionMultiplexer _connectionMultiplexer = null!;

    public SeaConnectionMultiplexer(string connectionString)
    {
        lock (_locker)
        {
            if (_connectionMultiplexer is not null)
            {
                return;
            }

            _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
        }
    }

    IConnectionMultiplexer ISeaConnectionMultiplexer.Instance => _connectionMultiplexer;
}
