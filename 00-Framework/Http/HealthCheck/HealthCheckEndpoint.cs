namespace PubSea.Framework.Http.HealthCheck;

public sealed class HealthCheckEndpoint
{
    public required string Name { get; init; }
    public required string Url { get; init; }
}
