namespace PubSea.Framework.Attrubutes;

/// <summary>
/// Sea rate limit attribute taking time window in seconds and 
/// max requests in the specified time window
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class SeaRateLimitAttribute : Attribute
{
    public int TimeWindowInSeconds { get; set; }
    public int MaxRequests { get; set; }
    public string? ErrorMessage { get; set; }
}

