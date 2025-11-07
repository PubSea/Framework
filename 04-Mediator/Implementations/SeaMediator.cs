using Microsoft.Extensions.DependencyInjection;
using PubSea.Mediator.Abstractions;

namespace PubSea.Mediator.Implementations;

internal sealed class SeaMediator : ISeaMediator
{
    private readonly IServiceProvider _serviceProvider;

    public SeaMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    Task ISeaMediator.Send<TReq>(TReq request, CancellationToken ct)
    {
        var handler = _serviceProvider.GetRequiredService<ISeaRequestHandler<TReq>>();
        return handler.Handle(request, ct);
    }

    Task<TRes> ISeaMediator.Send<TReq, TRes>(TReq request, CancellationToken ct)
    {
        var handler = _serviceProvider.GetRequiredService<ISeaRequestHandler<TReq, TRes>>();
        return handler.Handle(request, ct);
    }

    Task ISeaMediator.Publish<TReq>(TReq request, CancellationToken ct)
    {
        var handlers = _serviceProvider.GetServices<ISeaRequestHandler<TReq>>();
        List<Task> tasks = new(handlers.Count());
        foreach (var handler in handlers)
        {
            tasks.Add(handler.Handle(request, ct));
        }

        return Task.WhenAll(tasks);
    }
}
