using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PubSea.Framework.ApiResponse;
using PubSea.Framework.Exceptions;
using PubSea.Framework.Utility;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PubSea.Framework.Middlewares;

/// <summary>
/// This is a middleware which can be used to catch and handle errors raised in the application 
/// before going out of the app. It is beneficial to use this middleware in http rest apis.
/// </summary>
public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogger<ExceptionMiddleware> logger)
    {
        try
        {
            await _next(context);

            switch (context.Response.StatusCode)
            {
                case (int)HttpStatusCode.Unauthorized:
                    var unauthorizedResult = GenerateErrorJson("عدم احراز هویت", "401", 40_001);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(unauthorizedResult);
                    break;
                case (int)HttpStatusCode.Forbidden:
                    var forbiddenResult = GenerateErrorJson("عدم دسترسی", "403", 40_003);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(forbiddenResult);
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            var seaException = ex as SeaException;
            var message = GetErrorMessage(ex);
            var traceId = seaException?.TraceId ?? TraceIdGenerator.Generate();
            var code = seaException?.Code ?? SeaException.INTERNAL_ERROR_CODE;

            logger.LogErrorWithTraceId(ex, message, traceId);

            var result = GenerateErrorJson(message, traceId, code);

            context.Response.StatusCode = (int)StatusCodeFinder.FindHttpStatusCode(ex);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);
        }
    }

    private static string GetErrorMessage(Exception ex)
    {
        var message = ex switch
        {
            RpcException rpce => rpce.Status.Detail,
            _ => ex is SeaException ? ex.Message : SeaException.BASE_MESSAGE,
        };

        return string.IsNullOrWhiteSpace(message) ? SeaException.BASE_MESSAGE : message;
    }

    private static string GenerateErrorJson(string message, string traceId, int code)
    {
        var apiError = new ApiError(message, traceId, code);
        var apiResult = ApiResult.From(apiError);

        JsonSerializerOptions jsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };
        var result = JsonSerializer.Serialize(apiResult, jsonSerializerOptions);
        return result;
    }
}
