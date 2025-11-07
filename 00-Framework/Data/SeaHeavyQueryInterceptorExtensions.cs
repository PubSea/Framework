using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PubSea.Framework.Data;

public static class SeaHeavyQueryInterceptorExtensions
{
    public static DbContextOptionsBuilder UseSeaHeavyQueryInterceptor(this DbContextOptionsBuilder options,
        IServiceProvider provider)
    {
        var heavyQueryLogger = provider.GetRequiredService<ILogger<HeavyQueryInterceptor>>();

        options.AddInterceptors(new HeavyQueryInterceptor(heavyQueryLogger));
        return options;
    }
}
