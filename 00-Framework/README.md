# PubSea.Framework

**ASP.NET Core building blocks for DDD services** — domain events, EF Core unit of work, unified API responses, global error handling, Snowflake/HashId ID generation, Mapster mapping, Redis hybrid cache, and cURL health checks.

Install this package when you need production-ready cross-cutting utilities for ASP.NET Core Web APIs. For CQRS/mediator use [`PubSea.Mediator`](https://www.nuget.org/packages/PubSea.Mediator). For transactional outbox/inbox messaging use [`PubSea.Messaging.EntityFramework`](https://www.nuget.org/packages/PubSea.Messaging.EntityFramework).

## Install

```bash
dotnet add package PubSea.Framework
```

Requires **.NET 10** (ASP.NET Core).

## Quick start

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSeaEventDispatcher();  // domain events + auto-discovered handlers
builder.Services.AddSeaMapper();           // Mapster-based mapping
builder.Services.AddEfUnitOfWork<AppDbContext>();
builder.Services.AddSnowflakeService();
builder.Services.AddHashIdService();

var app = builder.Build();

app.UseSeaMapper();
app.UseSeaEventDispatcher();
app.UseMiddleware<WebErrorHandlerMiddleware>();

app.MapControllers();
app.Run();
```

## What you get

| Area | APIs |
|------|------|
| **DDD** | `Entity<T>`, `AggregateRoot<T>`, `IAuditable`, `IEvent`, `IDomainEvent`, `IEventHandler<T>`, `IEventDispatcher` |
| **Unit of work** | `AddEfUnitOfWork<TDbContext>()`, `IEfUnitOfWork` with transactions and execution strategy |
| **API responses** | `ToActionResult()` extensions — consistent `{ result: ... }` envelope and 201 Created helpers |
| **Error handling** | `WebErrorHandlerMiddleware`, `SeaException` with trace IDs and normalized problem payloads |
| **Mapping** | `AddSeaMapper()`, `ISeaMapper`, `SeaTypeAdapterConfig` (Mapster) |
| **IDs** | `ISnowflakeService` (time-ordered IDs), `IHashIdService` (Sqids encode/decode) |
| **Health checks** | `AddCurlHealthCheck()` — synthetic HTTP checks with aggressive timeouts |
| **HTTP debugging** | `HttpErrorLoggerMessageHandler` — logs failed outbound calls as reproducible cURL |
| **Caching** | `AddSeaRedisHybridCache()` — Microsoft Hybrid Cache + StackExchange.Redis |
| **Session logout (SLO)** | `AddSeaIdentityProviderSlo()` + `TerminatedSessionsMiddleware` |
| **Date/time** | `IDateTimeService` with Persian (Jalali) calendar conversion |
| **Files** | `ISeaFileService` — save files and build presigned URLs |
| **EF diagnostics** | `AddSeaHeavyQueryInterceptor()` — log slow queries |

## Examples

### Unified API responses

```csharp
[HttpGet("{id}")]
public IActionResult Get(long id)
    => new { id, email = "user@example.com" }.ToActionResult();

[HttpPost]
public IActionResult Create([FromBody] CreateUserRequest req)
    => new { id = 1L }.ToActionResult("/users/1");  // 201 Created
```

### Domain events

```csharp
public sealed class UserCreated : IEnrichedDomainEvent
{
    public long UserId { get; init; }
    public long EventId { get; set; }
    public DateTime PublishedUtcDateTime { get; set; }
}

public sealed class SendWelcomeEmailHandler : IEventHandler<UserCreated>
{
    public Task Handle(UserCreated evt, CancellationToken ct = default)
        => Task.CompletedTask;
}

// After SaveChanges:
SeaEventHelper.InitializeEvents(evt);
await eventDispatcher.Dispatch([evt], ct);
```

### Snowflake + HashId

```csharp
builder.Services.AddSnowflakeService(o => o.GeneratorId = 1);
builder.Services.AddHashIdService(o => o.MinHashLength = 12);

var id = snowflake.CreateId();
var publicId = hashId.Encode(id);
```

## Related packages

| Package | Use when |
|---------|----------|
| [PubSea.Mediator](https://www.nuget.org/packages/PubSea.Mediator) | CQRS commands/queries with `Send` / `Publish` |
| [PubSea.Messaging.EntityFramework](https://www.nuget.org/packages/PubSea.Messaging.EntityFramework) | EF Core outbox/inbox for reliable messaging |
| [PubSea.Messaging.EntityFramework.Kafka](https://www.nuget.org/packages/PubSea.Messaging.EntityFramework.Kafka) | Kafka transport |
| [PubSea.Messaging.EntityFramework.RabbitMq](https://www.nuget.org/packages/PubSea.Messaging.EntityFramework.RabbitMq) | RabbitMQ transport |

## Full documentation

Complete guides, samples, and architecture notes: [github.com/PubSea/Framework](https://github.com/PubSea/Framework)

Sample app: [`Qualifiers/00-RestApi`](https://github.com/PubSea/Framework/tree/main/Qualifiers/00-RestApi)

## License

MIT
