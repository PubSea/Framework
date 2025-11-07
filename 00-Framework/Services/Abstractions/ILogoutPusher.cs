namespace PubSea.Framework.Services.Abstractions;

public interface ILogoutPusher
{
    /// <summary>
    /// Pushes logout event to Redis
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task Push(string sessionId, CancellationToken ct = default);
}
