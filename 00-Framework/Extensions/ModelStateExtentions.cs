using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PubSea.Framework.ApiResponse;
using PubSea.Framework.Exceptions;
using System.Net;

namespace PubSea.Framework.Extensions;

public static class ModelStateExtentions
{
    public static IActionResult ToFormErrorResult(this ModelStateDictionary modelState,
        SeaException? seaException = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        var states = modelState.Where(s => s.Value?.ValidationState == ModelValidationState.Invalid)
            .ToDictionary(s => s.Key, s => s.Value?.Errors.Select(e => e.ErrorMessage));

        var code = seaException?.Code ?? SeaException.INVALID_REQUEST_CODE;
        var result = new FormError
        {
            Message = seaException?.Message ?? "اطلاعات وارد شده اشتباه است",
            Code = code,
            TraceId = code.ToString(),
            States = states!,
        };

        return new ObjectResult(ApiResult.From(result))
        {
            StatusCode = (int)statusCode
        };
    }
}

file class FormError
{
    public string Message { get; init; } = null!;
    public string TraceId { get; init; } = null!;
    public int Code { get; init; }
    public IDictionary<string, IEnumerable<string>> States { get; set; } = new Dictionary<string, IEnumerable<string>>();
}
