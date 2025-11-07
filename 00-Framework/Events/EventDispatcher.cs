using Microsoft.Extensions.DependencyInjection;

namespace PubSea.Framework.Events;

internal sealed class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _provider;

    public EventDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    async Task IEventDispatcher.Dispatch<TEvent>(TEvent[] events, CancellationToken ct)
    {
        foreach (var @event in events)
        {
            var handlers = _provider.GetServices<IEventHandler<TEvent>>();
            var tasks = new List<Task>();
            foreach (var handler in handlers)
            {
                tasks.Add(handler.Handle(@event, ct));
            }

            await Task.WhenAll(tasks);
        }
    }
}
