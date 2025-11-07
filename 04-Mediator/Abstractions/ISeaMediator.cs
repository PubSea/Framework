namespace PubSea.Mediator.Abstractions;

public interface ISeaMediator
{
    Task Send<TReq>(TReq request, CancellationToken ct = default)
        where TReq : ISeaRequest;

    Task<TRes> Send<TReq, TRes>(TReq request, CancellationToken ct = default)
        where TReq : ISeaRequest<TRes>;

    Task Publish<TReq>(TReq request, CancellationToken ct = default)
        where TReq : ISeaRequest;
}
