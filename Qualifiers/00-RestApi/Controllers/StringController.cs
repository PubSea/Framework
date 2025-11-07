using PubSea.Framework.Utility;
using Microsoft.AspNetCore.Mvc;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class StringController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var text = "پوژيا٢٢٨٠٠٠٢";
        return Ok(new { Original = text, Transformed = text.CorrectSpelling(), });
    }

    [HttpGet("check-differences")]
    public IActionResult Differences()
    {
        var text = "پوژیا";
        var text2 = "پوژيا";
        return Ok(string.Equals(text, text2, StringComparison.OrdinalIgnoreCase));
    }
}