namespace PubSea.Framework.Services.Implementations;

public sealed class HashIdOptions
{
    /// <summary>
    /// The salt is used for encoding and decoding. If you are going to change it, you need to 
    /// make sure to keep it safe and unchangable in order to keep consistency.
    /// </summary>
    public string Salt { get; set; } = "Sea Hash Id Generator";

    /// <summary>
    /// Minimum length of generated hash 
    /// </summary>
    public byte MinHashLength { get; set; } = 11;
}