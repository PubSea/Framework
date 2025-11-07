namespace PubSea.Mediator.Abstractions;

public interface ISeaRequestHandler<in TRequest>
    where TRequest : ISeaRequest
{
    Task Handle(TRequest request, CancellationToken ct = default);
}

public interface ISeaRequestHandler<in TRequest, TResponse>
    where TRequest : ISeaRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken ct = default);
}