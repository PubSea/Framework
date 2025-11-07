using Microsoft.AspNetCore.Mvc;
using PubSea.Framework.Exceptions;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class ExceptionController : ControllerBase
{
    [HttpGet("internal-error")]
    public IActionResult GetInternalException()
    {
        throw new Exception("Invalid");
    }

    [HttpGet("not-found")]
    public IActionResult GetNotFoundExceptoin()
    {
        throw new SeaException("Custom exception", SeaException.NOT_FOUND_CODE, ExceptionStatus.NotFound);
    }

    [HttpGet("/api/error")]
    public IActionResult MakeError()
    {
        throw new SeaException("Custom exception", SeaException.NOT_FOUND_CODE, ExceptionStatus.NotFound);
    }
}