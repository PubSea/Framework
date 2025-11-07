using Mapster;
using MapsterMapper;

namespace PubSea.Framework.Mapping;

internal sealed class SeaMapper : ServiceMapper, ISeaMapper
{
    public SeaMapper(IServiceProvider serviceProvider, TypeAdapterConfig config)
        : base(serviceProvider, config)
    { }
}
