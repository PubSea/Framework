using MessagingEfKafka.Events;
using Microsoft.AspNetCore.Mvc;
using PubSea.Framework.Data;
using PubSea.Messaging.EntityFramework.Services.Abstractions;
using PubSea.Framework.Services.Abstractions;

namespace MessagingEfKafka.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagingController : ControllerBase
{
    [HttpPost("/simple")]
    public async Task<IActionResult> Get([FromServices] ISeaPublisher publisher, [FromServices] IUnitOfWork unitOfWork,
        [FromServices] AppDbContext dbContext, [FromServices] ISnowflakeService snowflakeService, CancellationToken ct)
    {
        var id = snowflakeService.CreateId();
        var profile = Profile.Create(id, "خشایار", "مستوفی زاده");
        await dbContext.Profiles.AddAsync(profile, ct);
        await publisher.Publish(profile.GetEvents(), "Farshad Goodarzi", ct);

        await unitOfWork.SaveChanges(ct);

        return Ok("Done");
    }

    [HttpPost("/complex")]
    public async Task<IActionResult> TestComplext([FromServices] ISeaPublisher publisher,
        [FromServices] IUnitOfWork unitOfWork,
        [FromServices] ISnowflakeService snowflakeService, CancellationToken ct)
    {
        var complexEvent = new ComplexMessage
        {
            InsideMessage = new()
            {
                FullName = "farshad goodarzi",
                UniqueIdentifier = 45618715,
            },
            String = ["ali", "reza",],
            Numbers = [1, 2, 3,],
            InterfaceNumbers = [4, 5, 6,],
            Dict = new Dictionary<long, string>
            {
                { 1, "ali" },
                { 2, "reza" },
                { 3, "hasan" },
            },
            InterfaceDict = new Dictionary<long, string>
            {
                { 4, "ali" },
                { 5, "reza" },
                { 6, "hasan" },
            },
        };
        await publisher.Publish([complexEvent,], "Farshad Goodarzi", ct);

        await unitOfWork.SaveChanges(ct);

        return Ok("Done");
    }

    [HttpPost("/direct")]
    public async Task<IActionResult> Direct([FromServices] ISeaPublisher publisher,
        [FromServices] ISnowflakeService snowflakeService, CancellationToken ct)
    {
        var id = snowflakeService.CreateId();
        var profile = Profile.Create(id, "خشایار", "مستوفی زاده");
        await publisher.PublishDirectly(profile.GetEvents(), ct);

        return Ok("Done");
    }
}
