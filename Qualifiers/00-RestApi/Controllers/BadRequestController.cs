using PubSea.Framework.ApiResponse;
using Microsoft.AspNetCore.Mvc;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class BadRequestController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] BadRequestModel bad, CancellationToken ct)
    {
        return bad.ToActionResult();
    }
}

public class BadRequestModel
{
    public BadRequestEnum Bad1 { get; set; }
    public BadRequestEnum Bad2 { get; set; }
}

public enum BadRequestEnum
{
    Valid = 10,
    Invalid = 20,
}