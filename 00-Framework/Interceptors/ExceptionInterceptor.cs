using PubSea.Framework.Exceptions;
using PubSea.Framework.Services.Abstractions;
using PubSea.Framework.Services.Implementations;
using PubSea.Framework.Utility;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace PubSea.Framework.Interceptors;

/// <summary>
/// This is an interceptor which can be used to catch and handle errors raised in the application 
/// before going out of the app. It is beneficial to use this interceptor in grpc services.
/// </summary>
public class ExceptionInterceptor : Interceptor
{
    private readonly ILogger<ExceptionInterceptor> _logger;

    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        return await AsyncHandle(async () => await base.UnaryServerHandler(request, context, continuation));
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        return await AsyncHandle(async () => await base.ClientStreamingServerHandler(requestStream, context, continuation));
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream, ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await AsyncHandle(async () => await base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation));
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request,
        IServerStreamWriter<TResponse> responseStream, ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await AsyncHandle(async () => await base.ServerStreamingServerHandler(request, responseStream, context, continuation));
    }

    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        return Handle(() => base.AsyncClientStreamingCall(context, continuation));
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest,
        TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        return Handle(() => base.AsyncDuplexStreamingCall(context, continuation));
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
        TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
        AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        return Handle(() => base.AsyncServerStreamingCall(request, context, continuation));
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        return Handle(() => base.AsyncUnaryCall(request, context, continuation));
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(
        TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
        BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        return Handle(() => base.BlockingUnaryCall(request, context, continuation));
    }

    private T Handle<T>(Func<T> handler)
    {
        try
        {
            return handler.Invoke();
        }
        catch (Exception ex)
        {
            var (message, metadata) = LogException(ex);
            throw new RpcException(new Status(StatusCodeFinder.FindGrpcStatusCode(ex), message), metadata, message);
        }
    }

    private async Task<T> AsyncHandle<T>(Func<Task<T>> handler)
    {
        try
        {
            return await handler.Invoke();
        }
        catch (Exception ex)
        {
            var (message, metadata) = LogException(ex);
            throw new RpcException(new Status(StatusCodeFinder.FindGrpcStatusCode(ex), message), metadata, message);
        }
    }

    private async Task AsyncHandle(Func<Task> handler)
    {
        try
        {
            await handler.Invoke();
        }
        catch (Exception ex)
        {
            var (message, metadata) = LogException(ex);
            throw new RpcException(new Status(StatusCodeFinder.FindGrpcStatusCode(ex), message), metadata, message);
        }
    }

    private (string Message, Metadata Metadata) LogException(Exception ex)
    {
        var message = string.IsNullOrWhiteSpace(ex.Message) ? SeaException.BASE_MESSAGE : ex.Message;
        var traceId = ex is SeaException seaEx ? seaEx.TraceId : TraceIdGenerator.Generate();

        _logger.LogErrorWithTraceId(ex, message, traceId);

        Metadata metadata = [];
        if (!string.IsNullOrWhiteSpace(traceId))
        {
            metadata.Add("TraceId", traceId);
        }

        return (message, metadata);
    }
}
