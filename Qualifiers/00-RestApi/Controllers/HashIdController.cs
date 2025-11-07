using PubSea.Framework.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class HashIdController : ControllerBase
{
    private readonly IHashIdService _hashIdService;

    public HashIdController(IHashIdService hashIdService)
    {
        _hashIdService = hashIdService;
    }

    [HttpGet("encode/{id:int}")]
    public IActionResult Encode([FromRoute] int id)
    {
        var hash = _hashIdService.Encode(id);
        return Ok(hash);
    }

    [HttpGet("decode/{hash}")]
    public IActionResult Decode([FromRoute] string hash)
    {
        var id = _hashIdService.Decode(hash);
        return Ok(id.ToString());
    }

    [HttpGet("encode-long/{id:long}")]
    public IActionResult EncodeLong([FromRoute] long id)
    {
        var hash = _hashIdService.EncodeLong(id);
        return Ok(hash);
    }

    [HttpGet("decode-long/{hash}")]
    public IActionResult DecodeLong([FromRoute] string hash)
    {
        var id = _hashIdService.DecodeLong(hash);
        return Ok(id.ToString());
    }
}