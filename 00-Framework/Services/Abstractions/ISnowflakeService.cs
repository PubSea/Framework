namespace PubSea.Framework.Services.Abstractions;

/// <summary>
/// This service creates a unique id each time it is called. To see more information about Twitter snowflake id 
/// please refer to https://en.wikipedia.org/wiki/Snowflake_ID
/// </summary>
public interface ISnowflakeService
{
    /// <summary>
    /// Creates a snowflake id
    /// </summary>
    /// <returns></returns>
    long CreateId();
}
