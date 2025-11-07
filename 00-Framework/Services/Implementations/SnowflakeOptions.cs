namespace PubSea.Framework.Services.Implementations;

public sealed class SnowflakeOptions
{
    /// <summary>
    /// Defines id for the instance responsible for generating id
    /// </summary>
    public int GeneratorId { get; set; } = 0;

    /// <summary>
    /// The base time to start counting
    /// </summary>
    public DateTime Epoch { get; set; } = new DateTime(2022, 9, 18, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Structure to define parts of id including timestamp bits, generator id bits and sequence bits
    /// </summary>
    public (byte timestampBits, byte generatorIdBits, byte sequenceBits) IdStructure { get; set; } = (43, 8, 12);
}
