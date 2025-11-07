using PubSea.Framework.Services.Abstractions;
using IdGen;

namespace PubSea.Framework.Services.Implementations;

internal sealed class SnowflakeService : ISnowflakeService
{
    private readonly IdGenerator _snowflake;

    internal SnowflakeService(SnowflakeOptions options)
    {
        var (timestampBits, generatorIdBits, sequenceBits) = options.IdStructure;
        var structure = new IdStructure(timestampBits, generatorIdBits, sequenceBits);
        var idGenOptions = new IdGeneratorOptions(structure, new DefaultTimeSource(options.Epoch));

        _snowflake = new IdGenerator(options.GeneratorId, idGenOptions);
    }

    long ISnowflakeService.CreateId()
    {
        return _snowflake.CreateId();
    }
}
