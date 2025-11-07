using PubSea.Framework.Events;
using PubSea.Framework.Utility;
using Microsoft.AspNetCore.Mvc;
using RestApi.Events;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class InternalEventController : ControllerBase
{
    private readonly IEventDispatcher _eventDispatcher;

    public InternalEventController(IEventDispatcher eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;
    }

    [HttpGet("creation")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var @event = new UserCreated();
        SeaEventHelper.InitializeEvents(@event);
        await _eventDispatcher.Dispatch(new[] { @event }, ct);
        return Ok();
    }

    [HttpGet("modification")]
    public async Task<IActionResult> Modify(CancellationToken ct)
    {
        var @event = new UserModified
        {
            UserId = 20,
        };
        SeaEventHelper.InitializeEvents(@event);
        await _eventDispatcher.Dispatch(new[] { @event }, ct);
        return Ok();
    }
}