using Microsoft.Extensions.DependencyInjection;
using PubSea.Framework.Extensions;
using PubSea.Framework.Services.Abstractions;
using StackExchange.Redis;

namespace PubSea.Framework.Services.Implementations;

internal sealed class LogoutPusher : ILogoutPusher
{
    private readonly IDatabase _db;

    public LogoutPusher([FromKeyedServices(SeaSloExtensions.LOGOUT_CONNECTION_MULTIPLEXER)] IConnectionMultiplexer cm)
    {
        _db = cm.GetDatabase(SeaSloExtensions.LOGOUT_CONNECTION_MULTIPLEXER_DB);
    }

    async Task ILogoutPusher.Push(string sessionId, CancellationToken ct)
    {
        var key = $"{SeaSloExtensions.LOGOUT_CONNECTION_MULTIPLEXER_PREFIX}{sessionId}";
        await _db.StringSetAsync(key, "1", TimeSpan.FromDays(10))
            .WaitAsync(ct);
    }
}
