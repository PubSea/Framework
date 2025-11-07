using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using PubSea.Messaging.EntityFramework.Configs;
using PubSea.Messaging.EntityFramework.RabbitMq.Configs;
using PubSea.Messaging.EntityFramework.RabbitMq.Services.Abstractions;

namespace PubSea.Messaging.EntityFramework.RabbitMq.Services.Implementations;

internal sealed class RabbitMqClient : IRabbitMqClient, IDisposable, IAsyncDisposable
{
    private static readonly SemaphoreSlim _lock = new(1, 1);

    private IConnection _connection = null!;
    private IChannel _channel = null!;

    private readonly ILogger<RabbitMqClient> _logger;
    private readonly SeaMessagingConfig _config;
    private readonly RabbitMqConfig _rabbitMqConfig;

    public RabbitMqClient(ILogger<RabbitMqClient> logger,
        SeaMessagingConfig config, RabbitMqConfig rabbitMqConfig)
    {
        _logger = logger;
        _config = config;
        _rabbitMqConfig = rabbitMqConfig;
    }

    async Task<IChannel> IRabbitMqClient.GetChannel(CancellationToken ct)
    {
        if (_channel is not null && _channel.IsOpen)
        {
            return _channel;
        }

        await _lock.WaitAsync(ct);

        if (_channel is not null && _channel.IsOpen)
        {
            return _channel;
        }

        var connection = await GetConnection(ct);
        _channel = await connection.CreateChannelAsync(cancellationToken: ct);

        _lock.Release();

        return _channel;
    }

    async Task<IChannel> IRabbitMqClient.GetNewChannel(CancellationToken ct)
    {
        var connection = await GetNewConnection(ct);
        return await connection.CreateChannelAsync(cancellationToken: ct);
    }

    private async Task<IConnection> GetConnection(CancellationToken ct)
    {
        if (_connection is not null && _connection.IsOpen)
        {
            return _connection;
        }

        _connection = await Connect(_logger, _rabbitMqConfig, retryCount: 3, ct);
        return _connection;
    }

    private async Task<IConnection> GetNewConnection(CancellationToken ct)
    {
        return await Connect(_logger, _rabbitMqConfig, retryCount: 3, ct);
    }

    private static async Task<IConnection> Connect(ILogger<RabbitMqClient> logger,
        RabbitMqConfig rabbitMqConfig, short retryCount, CancellationToken ct)
    {
        try
        {
            ConnectionFactory factory = new()
            {
                Uri = new Uri(rabbitMqConfig.ConnectionString),
                ClientProvidedName = rabbitMqConfig.ClientId,
            };

            var connection = await factory.CreateConnectionAsync(ct);

            logger.LogInformation("Connection to RabbitMq server established successfully {StartDate}",
                DateTime.UtcNow);

            return connection;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Connection to RabbitMq server faced to an error. Next try will be in 5 seconds {StartDate}",
                DateTime.UtcNow);

            retryCount--;
            if (retryCount <= 0)
            {
                throw;
            }

            var oneSecond = 1000;
            await Task.Delay(oneSecond, ct);

            return await Connect(logger, rabbitMqConfig, retryCount, ct);
        }
    }

    public void Dispose()
    {
        if (_connection.IsOpen)
        {
            _connection.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection.IsOpen)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}
