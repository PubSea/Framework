using Microsoft.AspNetCore.Mvc;
using PubSea.Mediator.Abstractions;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class MediatorController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] ISeaMediator mediator, CancellationToken ct)
    {
        GetAuthorsQuery query = new();
        var author = await mediator.Send<GetAuthorsQuery, Author>(query, ct);

        return Ok(author);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromServices] ISeaMediator mediator, CancellationToken ct)
    {
        AddAuthorCommand command = new()
        {
            Author = new()
            {
                FirstName = "Lionel" + Random.Shared.Next(1, 1000),
                LastName = "Messi" + Random.Shared.Next(1, 1000),

            },
        };

        await mediator.Send(command, ct);
        return Ok();
    }
}

public sealed class Author
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}

public class GetAuthorsQuery : ISeaRequest<Author>
{ }

public sealed class GetAuthorsQueryHandler : ISeaRequestHandler<GetAuthorsQuery, Author>
{
    async Task<Author> ISeaRequestHandler<GetAuthorsQuery, Author>
        .Handle(GetAuthorsQuery request, CancellationToken ct)
    {
        await Task.CompletedTask;
        return new Author
        {
            FirstName = "Lionel",
            LastName = "Messi",
        };
    }
}

public sealed class AddAuthorCommand : ISeaRequest
{
    public Author Author { get; set; } = null!;
}

public sealed class AddAuthorCommandHandler : ISeaRequestHandler<AddAuthorCommand>
{
    private static readonly List<Author> _authors = [];

    async Task ISeaRequestHandler<AddAuthorCommand>
        .Handle(AddAuthorCommand rq, CancellationToken ct)
    {
        _authors.Add(rq.Author);
        await Task.CompletedTask;
    }
}
