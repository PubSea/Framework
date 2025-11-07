using PubSea.Framework.Exceptions;
using PubSea.Framework.Utility;
using Microsoft.AspNetCore.Mvc;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;

    public LogController(ILogger<LogController> logger)
    {
        _logger = logger;
    }

    [HttpGet("log")]
    public IActionResult Log()
    {
        var ex = new SeaException("Hello this is just a log message", ExceptionStatus.NotFound);
        _logger.LogEnriched(LogLevel.Error, ex.Message, ex);
        return Ok();
    }

    [HttpGet("throw")]
    public IActionResult Throw()
    {
        throw new SeaException("Hello this is just a throw message", ExceptionStatus.NotFound);
    }

    [HttpGet("log-info")]
    public IActionResult LogInfo()
    {
        _logger.LogEnriched(LogLevel.Information, "Hi {FirstName} {LastName}", "Lionel", "Messi");

        return Ok();
    }
}