using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PubSea.Framework.ApiResponse;
using PubSea.Framework.Exceptions;
using PubSea.Framework.Utility;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PubSea.Framework.Middlewares;

public class WebErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public WebErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment web)
    {
        try
        {
            await _next(context);

            bool ifErrorRaised = true;
            var seaException = new SeaException();

            switch (context.Response.StatusCode)
            {
                case (int)HttpStatusCode.OK:
                    ifErrorRaised = false;
                    break;
                case (int)HttpStatusCode.NotFound:
                    seaException = new SeaException($"Requested address not found - {context.Request.Path.Value}",
                        SeaException.NOT_FOUND_CODE, ExceptionStatus.NotFound);
                    break;
                case (int)HttpStatusCode.Unauthorized:
                    seaException = new SeaException($"Unauthorized - {context.Request.Path.Value}",
                        SeaException.UNAUTHENTICATED_CODE, ExceptionStatus.Unauthenticated);
                    break;
                case (int)HttpStatusCode.Forbidden:
                    seaException = new SeaException($"Permission denied - {context.Request.Path.Value}",
                        SeaException.FORBIDDEN_CODE, ExceptionStatus.PermissionDenied);
                    break;
                case (int)HttpStatusCode.TooManyRequests:
                    seaException = new SeaException($"Too many requests - {context.Request.Path.Value}",
                        SeaException.TOO_MANY_REQUEST_CODE, ExceptionStatus.TooManyRequests);
                    break;
                default:
                    ifErrorRaised = false;
                    break;
            }

            if (ifErrorRaised)
            {
                logger.LogException(seaException, seaException.Message, seaException.TraceId, seaException.Code);

                if (context.Request.Path.ToString().StartsWith("/api/", StringComparison.OrdinalIgnoreCase) ||
                    (context.Request.Headers.Accept.FirstOrDefault()?.Contains("json") ?? false))
                {
                    await GenerateApiError(context, seaException);
                }
                else
                {
                    await GenerateErrorPage(context, web, seaException);
                }
            }
        }
        catch (Exception ex)
        {
            var seaException = ex as SeaException ?? new SeaException(SeaException.BASE_MESSAGE, ex);
            logger.LogException(seaException, seaException.Message, seaException.TraceId, seaException.Code);

            var formContextType = context.Request.ContentType?.Contains("form") ?? false;
            if (formContextType && ExceptionHelper.ExcludedExCodes.Contains(seaException.Code))
            {
                seaException = new SeaException("اطلاعات وارد شده اشتباه است",
                    seaException.Code, ExceptionStatus.InvalidArgument);
            }

            var isFormPostback = context.Request.Query.TryGetValue("postback", out _);
            if (context.Request.Path.ToString().StartsWith("/api/", StringComparison.OrdinalIgnoreCase) ||
                (context.Request.Headers.Accept.FirstOrDefault()?.Contains("json") ?? false) ||
                (formContextType && !isFormPostback))
            {
                await GenerateApiError(context, seaException);
            }
            else
            {
                await GenerateErrorPage(context, web, seaException);
            }
        }
    }

    private static async Task GenerateApiError(HttpContext context, SeaException ex)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = (int)StatusCodeFinder.FindHttpStatusCode(ex);

        var apiError = new ApiError(ex.Message, ex.TraceId, ex.Code);
        var apiResult = ApiResult.From(apiError);
        await context.Response.WriteAsync(JsonSerializer.Serialize(apiResult, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        }));
    }

    private static async Task GenerateErrorPage(HttpContext context, IWebHostEnvironment web, SeaException seaException)
    {
        var pageName = seaException.ExceptionStatus switch
        {
            ExceptionStatus.Unauthenticated => "401.html",
            ExceptionStatus.PermissionDenied => "403.html",
            ExceptionStatus.NotFound => "404.html",
            ExceptionStatus.TooManyRequests => "429.html",
            _ => "500.html",
        };

        var errorPage = (await File.ReadAllTextAsync($"{web.WebRootPath}/pages/{pageName}"))
            .Replace("@@errorcode@@", seaException.TraceId.ToString());

        var statusCode = (int)StatusCodeFinder.FindHttpStatusCode(seaException);
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(errorPage);
    }
}