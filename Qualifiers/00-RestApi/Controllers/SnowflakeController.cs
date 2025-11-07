using PubSea.Framework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class SnowflakeController : ControllerBase
{
    private readonly ISnowflakeService _snowflakeService;

    public SnowflakeController(ISnowflakeService snowflakeService)
    {
        _snowflakeService = snowflakeService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var id = _snowflakeService.CreateId();
        return Ok(id);
    }
}