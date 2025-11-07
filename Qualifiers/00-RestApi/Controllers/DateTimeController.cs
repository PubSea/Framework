using Microsoft.AspNetCore.Mvc;
using PubSea.Framework.Services.Abstractions;
using PubSea.Framework.Utility;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class DateTimeController : ControllerBase
{
    [HttpGet("to-persian-datetime")]
    public IActionResult ToPersianDateTime()
    {
        var dt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        var date = dt.ToJalaliDateTimeString();
        return Ok(date);
    }

    [HttpGet("to-persian-date")]
    public IActionResult ToPersianDate([FromServices] IDateTimeService dateTimeService)
    {
        var date = dateTimeService.ToPersianDate(DateTime.Now);
        return Ok(date.ToString());
    }

    [HttpGet("from-persian-date")]
    public IActionResult FromPersianDate([FromServices] IDateTimeService dateTimeService)
    {
        var date = dateTimeService.FromPersianDate(1401, 7, 10, 0, 0, 0, 0);
        return Ok(date.UtcDateTime);
    }
}