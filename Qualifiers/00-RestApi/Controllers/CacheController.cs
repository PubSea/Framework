using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using PubSea.Framework.Services.Abstractions;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class CacheController : ControllerBase
{
    [HttpGet("Connection-Multiplexer")]
    public async Task<IActionResult> GetByConnectionMultiplexer(
        [FromServices] ISeaConnectionMultiplexer connectionMultiplexer,
        CancellationToken ct)
    {
        var db = connectionMultiplexer.Instance.GetDatabase(0);
        var key = "programmer";

        await db.HashSetAsync(key, "first_name", "Dennis").WaitAsync(ct);
        await db.HashSetAsync(key, "last_name", "Ritchie").WaitAsync(ct);
        await db.HashSetAsync(key, "full_name", "Dennis Ritchie").WaitAsync(ct);

        var programmer = await db.HashGetAllAsync(key).WaitAsync(ct);

        Dictionary<string, string> entry = [];
        foreach (var item in programmer)
        {
            entry.Add(item.Name!, item.Value!);
        }

        return Ok(entry);
    }

    [HttpGet()]
    public async Task<IActionResult> Get([FromServices] HybridCache cache, CancellationToken ct)
    {
        var player = await cache.GetOrCreateAsync("player",
        token =>
        {
            Player p = new()
            {
                FirstName = "Lionel",
                LastName = "Messi",
            };

            return ValueTask.FromResult(p);
        }, cancellationToken: ct);

        return Ok(player);
    }
}

public class Player
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}