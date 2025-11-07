using PubSea.Framework.Attrubutes;
using Microsoft.AspNetCore.Mvc;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class RateLimitController : ControllerBase
{
    [HttpPost("/crl")]
    [SeaRateLimit(MaxRequests = 2, TimeWindowInSeconds = 120, ErrorMessage = "Hello")]
    public IActionResult Get()
    {
        return Ok(1);
    }
}