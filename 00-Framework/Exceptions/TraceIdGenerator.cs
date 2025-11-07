using PubSea.Framework.Services.Abstractions;
using PubSea.Framework.Services.Implementations;

namespace PubSea.Framework.Exceptions;

public static class TraceIdGenerator
{
    private static readonly ISnowflakeService _snowflakeService = new SnowflakeService(new SnowflakeOptions
    {
        Epoch = new DateTime(2023, 1, 1),
        GeneratorId = 255,
        IdStructure = (43, 8, 12),
    });

    private static readonly IHashIdService _hashIdService = new HashIdService(new HashIdOptions
    {
        MinHashLength = 6,
        Salt = "TraceId",
    });

    public static string Generate()
    {
        var id = _snowflakeService.CreateId();
        return _hashIdService.EncodeLong(id).ToLower();
    }
}
