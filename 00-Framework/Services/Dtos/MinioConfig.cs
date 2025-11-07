namespace PubSea.Framework.Services.Dtos;

public sealed class SeaFileStoreConfig
{
    /// <summary>
    /// Base url of object storage
    /// </summary>
    public string BaseUrl { get; set; } = default!;

    /// <summary>
    /// UserName to connect to object storage
    /// </summary>
    public string UserName { get; set; } = default!;

    /// <summary>
    /// Password to connect to object storage
    /// </summary>
    public string Password { get; set; } = default!;

    /// <summary>
    /// Name of root element which other elements originate from it. For storage like Minio 
    /// this is the bucket name
    /// </summary>
    public string RootName { get; set; } = default!;
}
