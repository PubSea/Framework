using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PubSea.Framework.ApiResponse;
using PubSea.Framework.Services.Abstractions;

namespace PubSea.Framework.Attributes;

/// <summary>
/// Reforms default input errors and turn them in <see cref="ApiError"/>.
/// </summary>
public sealed class ReformatInputValidationAttribute : ActionFilterAttribute
{
    private readonly ISnowflakeService _snowflakeService;
    private readonly IHashIdService _hashIdService;
    private readonly ILogger<ReformatInputValidationAttribute> _logger;

    public ReformatInputValidationAttribute(ISnowflakeService snowflakeService, IHashIdService hashIdService,
        ILogger<ReformatInputValidationAttribute> logger)
    {
        _snowflakeService = snowflakeService;
        _hashIdService = hashIdService;
        _logger = logger;
    }

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is BadRequestObjectResult badRequestObjectResult and { Value: ValidationProblemDetails })
        {
            var traceId = _hashIdService.EncodeLong(_snowflakeService.CreateId());
            var message = "تایپ ورودی (ها) نامعتبر است";
            var error = new ApiError
            {
                Message = message,
                TraceId = traceId,
                Code = 40_000,
            };
            var result = ApiResult.From(error);
            context.Result = new BadRequestObjectResult(result);
            _logger.LogError("Input validation error happened. {TraceId}, {Message}, {Error}",
                traceId, message, badRequestObjectResult);
        }

        base.OnResultExecuting(context);
    }
}
