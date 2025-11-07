using StackExchange.Redis;

namespace PubSea.Framework.Services.Abstractions;

public interface ISeaConnectionMultiplexer
{
    IConnectionMultiplexer Instance { get; }
}
