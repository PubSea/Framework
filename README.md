## PubSea Framework

### Table of Contents

- [Overview](#overview)
- [Solution Layout](#solution-layout)
- [NuGet Packages and Features](#nuget-packages-and-features)
- [Getting Started](#getting-started)
  - [Minimal wiring (Web API)](#minimal-wiring-web-api)
  - [Mediator](#mediator)
  - [Domain Events](#domain-events)
  - [Messaging (EF Outbox/Inbox)](#messaging-ef-outboxinbox)
  - [Kafka Provider](#kafka-provider)
  - [RabbitMQ Provider](#rabbitmq-provider)
  - [Additional Utilities](#additional-utilities-00-framework)
    - [API Responses](#api-responses)
    - [Error Handling](#error-handling-middleware)
    - [Health Checks](#health-checks-and-curl-checks)
    - [HttpClient Error Logging](#httpclient-error-logging)
    - [Mapping (Mapster)](#mapster-based-mapping)
    - [Snowflake & HashId](#snowflake-and-hashid-services)
    - [Unit of Work](#unit-of-work-with-ef-core)
    - [File Store (MinIO)](#file-store-minio)
- [Package Reference](#package-reference)
  - [00-Framework](#00-framework)
  - [01-Messaging.EntityFramework](#01-messagingentityframework)
  - [02-Messaging.EntityFramework.Kafka](#02-messagingentityframeworkkafka)
  - [03-Messaging.EntityFramework.RabbitMq](#03-messagingentityframeworkrabbitmq)
  - [04-Mediator](#04-mediator)
  - [Qualifiers (Samples)](#qualifiers-samples)
- [Design Notes](#design-notes)
- [Troubleshooting](#troubleshooting)
- [FAQ](#faq)
- [Build & Pack](#build--pack)
- [Contributing](#contributing)
- [License](#license)

### Overview

SeaFramework is an opinionated ASP.NET Core framework that applies Domain-Driven Design (DDD) patterns and provides production-ready utilities: a lightweight Mediator, an Entity Framework–backed Outbox/Inbox messaging module with pluggable brokers (Kafka, RabbitMQ, or a no‑op default), mapping helpers, health checks, error handling middleware, Snowflake/HashId services, file storage (MinIO), hybrid caching, and more.

Use packages together for a full stack, or reference only what you need.

### Solution Layout

- `00-Framework`: Core DDD and web utilities
  - Domain events and dispatcher (`IEvent`, `IEventHandler<T>`, `AddSeaEventDispatcher`)
  - Unit of Work (`IUnitOfWork`, `IEfUnitOfWork`, `AddEfUnitOfWork<TDbContext>`) and EF helpers
  - Middlewares (exception handler, web error handler) and API response helpers
  - Health checks (cURL checks), HTTP error logging handler
  - Mapster-based mapper (`AddSeaMapper`, `UseSeaMapper`)
  - Snowflake/HashId/DateTime services
  - File storage (MinIO) via `AddSeaFileStore`
  - Utilities: caching extensions, SLO helpers, logging, helpers

- `01-Messaging.EntityFramework`: EF Core Outbox/Inbox messaging
  - Outbox model and interceptor, inbox store, background services:
    - `OutboxPollingPublisher`, `OutboxMessageTransporter`, `InboxCleaner`
  - Auto‑discovery for `ISeaConsumer<TMessage>` implementations
  - Dynamic message contract registration for interface-based payloads

- `02-Messaging.EntityFramework.Kafka`: Kafka broker provider
- `03-Messaging.EntityFramework.RabbitMq`: RabbitMQ broker provider
- `04-Mediator`: Lightweight mediator with `Send`/`Publish` and handler discovery
- `Qualifiers/00-RestApi`: runnable sample for wiring the framework in an ASP.NET Core app
- `Qualifiers/01-MessagingEfKafka`: runnable sample for messaging with Kafka

## NuGet Packages and Features

Each numbered project is produced as a NuGet package. Install only what you need.

Packages:

- PubSea.Framework
  - PackageId: `PubSea.Framework`
  - Provides: DDD primitives, domain events/dispatcher, UoW, middlewares, API response helpers, mapper, health checks, HttpClient error logging, Snowflake/HashId/DateTime services, MinIO file store
  - Install:
    ```bash
    dotnet add package PubSea.Framework
    ```

- PubSea.Mediator
  - PackageId: `PubSea.Mediator`
  - Provides: Mediator abstractions and implementation; auto registration of handlers via DI
  - Install:
    ```bash
    dotnet add package PubSea.Mediator
    ```

- PubSea.Messaging.EntityFramework
  - PackageId: `PubSea.Messaging.EntityFramework`
  - Provides: EF Core Outbox/Inbox messaging, `ISeaPublisher`, background services, consumer discovery, EF interceptor
  - Depends on: `PubSea.Framework`
  - Install:
    ```bash
    dotnet add package PubSea.Messaging.EntityFramework
    ```

- PubSea.Messaging.EntityFramework.Kafka
  - PackageId: `PubSea.Messaging.EntityFramework.Kafka`
  - Provides: Kafka broker transport/consume integration for the messaging module
  - Depends on: `PubSea.Messaging.EntityFramework`
  - Install:
    ```bash
    dotnet add package PubSea.Messaging.EntityFramework.Kafka
    ```

- PubSea.Messaging.EntityFramework.RabbitMq
  - PackageId: `PubSea.Messaging.EntityFramework.RabbitMq`
  - Provides: RabbitMQ broker transport integration for the messaging module
  - Depends on: `PubSea.Messaging.EntityFramework`
  - Install:
    ```bash
    dotnet add package PubSea.Messaging.EntityFramework.RabbitMq
    ```

Feature to package mapping:

- Mediator (Send/Publish): `PubSea.Mediator`
- Domain events and dispatcher: `PubSea.Framework`
- API response envelope, middlewares, mapper, health checks, HttpClient logging, services (Snowflake/HashId/DateTime), MinIO file store: `PubSea.Framework`
- Messaging (Outbox/Inbox, `ISeaPublisher`, consumers, background services): `PubSea.Messaging.EntityFramework`
- Kafka transport: `PubSea.Messaging.EntityFramework.Kafka`
- RabbitMQ transport: `PubSea.Messaging.EntityFramework.RabbitMq`

---

## Getting started

Open the solution `SeaFramework.sln` in Visual Studio (or run `dotnet build`). You can reference projects directly or pack them as NuGet packages with `dotnet pack` (or `pack.bat`) and consume them from a local feed.

### Minimal wiring (Web API)

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Mediator
builder.Services.AddSeaMediator();

// Domain events (auto-registers IEventHandler<T>)
builder.Services.AddSeaEventDispatcher();

// Mapper (Mapster)
builder.Services.AddSeaMapper();

// Optional utilities
builder.Services.AddSnowflakeService();
builder.Services.AddHashIdService();
builder.Services.AddDateTimeService();

var app = builder.Build();

app.UseSeaMapper();
app.UseSeaEventDispatcher();

app.MapControllers();
app.Run();
```

### Mediator

```csharp
public sealed record CreateUser(string Email) : ISeaRequest;

public sealed class CreateUserHandler : ISeaRequestHandler<CreateUser>
{
    public Task Handle(CreateUser request, CancellationToken ct = default)
        => Task.CompletedTask;
}

public sealed record GetUser(long Id) : ISeaRequest<UserDto>;

public sealed class GetUserHandler : ISeaRequestHandler<GetUser, UserDto>
{
    public Task<UserDto> Handle(GetUser request, CancellationToken ct = default)
        => Task.FromResult(new UserDto());
}

await mediator.Send(new CreateUser("a@b.com"), ct);
var dto = await mediator.Send<GetUser, UserDto>(new GetUser(1), ct);
await mediator.Publish(new CreateUser("c@d.com"), ct); // fan-out
```

Handlers are auto-registered from loaded assemblies by `AddSeaMediator`.

### Domain Events

```csharp
builder.Services.AddSeaEventDispatcher();
app.UseSeaEventDispatcher();
```

Implement `IEventHandler<TEvent>` anywhere in your assemblies. Handlers are discovered and registered automatically.

### Messaging (EF Outbox/Inbox)

Add messaging and choose a broker provider. Your application `DbContext` is used for Outbox/Inbox; an EF interceptor persists outbox messages transactionally.

```csharp
builder.Services.AddSeaMessaging<AppDbContext>(cfg =>
{
    // For dev/local you can start with the default (no-op) broker:
    // cfg.UseDefaultBroker();

    // Tuning:
    // cfg.PublishOutboxInstantly = false;
    // cfg.OutboxPollingInterval = TimeSpan.FromSeconds(2);
    // cfg.InboxRetentionInterval = TimeSpan.FromMinutes(5);
    // cfg.InboxMessageTtl = TimeSpan.FromHours(24);
});
```

Publish events inside your application workflows:

```csharp
public sealed class UserCreated : IEvent { /* ... */ }

public sealed class UserService
{
    private readonly ISeaPublisher _publisher;
    private readonly ISeaMessagingDbContext _db; // your DbContext

    public async Task CreateUser(CancellationToken ct)
    {
        // ... persist user
        await _publisher.Publish([ new UserCreated(/*...*/) ], ct);
        await _db.SaveChangesAsync(ct); // outbox stored in same transaction
    }
}
```

Consume messages by implementing `ISeaConsumer<TMessage>`:

```csharp
public interface IUserCreated
{
    long Id { get; set; }
    string Email { get; set; }
}

public sealed class UserCreatedConsumer : ISeaConsumer<IUserCreated>
{
    public Task Consume(IUserCreated message, CancellationToken ct = default)
        => Task.CompletedTask;
}
```

Background services included:

- `OutboxPollingPublisher`: scans and enqueues unpublished outbox messages
- `OutboxMessageTransporter`: ships payloads to the configured broker
- `InboxCleaner`: retention/TTL of inbox messages

Implement your DbContext for messaging by implementing `ISeaMessagingDbContext`:

```csharp
public sealed class AppDbContext : DbContext, ISeaMessagingDbContext
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;
    public DbSet<ConsumedFaultMessage> ConsumedFaultMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Optional: override mappings for messaging tables if needed
        // modelBuilder.Entity<OutboxMessage>().ToTable("outbox_messages", "messaging");
    }
}
```

Notes:

- The EF interceptor (`OutboxSavedChangesInterceptor`) is wired automatically via `UseSeaMessaging` when you call `AddSeaMessaging<TDbContext>()`.
- `SeaPublisher.Publish(...)` stages outbox records; the actual broker transport happens by background services.
- If `PublishOutboxInstantly` is true, messages are sent to broker immediately during save; otherwise polling handles them.

### Kafka provider

```csharp
services.Configure<KafkaConfig>(cfg =>
{
    cfg.ClientId = "my-app";
    cfg.ConnectionString = "PLAINTEXT://localhost:9092";
    cfg.TopicName = "outbox";
    cfg.ConsumingTopicNames = new() { "outbox" };
    cfg.Partitions = 10;
    cfg.ConsumerGroupId = "my-app-group";
    cfg.ConcurrentConsumers = 2;
});
```

### RabbitMQ provider

```csharp
services.Configure<RabbitMqConfig>(cfg =>
{
    cfg.ClientId = "my-app";
    cfg.ConnectionString = "amqp://user:pass@localhost:5672";
});
```

---

## Additional utilities (00-Framework)
## Package Reference

### 00-Framework

- Namespaces and key entry points
  - `PubSea.Framework.DomainModel.FrameworkBootstarpper`
    - `AddSeaEventDispatcher()`, `UseSeaEventDispatcher()`
    - `AddSeaMapper()`, `UseSeaMapper()`
    - `AddEfUnitOfWork<TDbContext>()`
    - `AddSeaFileStore(Action<SeaFileStoreConfig>)`
    - `AddHttpErrorLoggerMessageHandler()`
  - Middlewares
    - `ExceptionMiddleware`, `WebErrorHandlerMiddleware`
  - API response
    - `ApiActionResult`, `ApiResult`, `ApiError`
  - Services
    - `ISnowflakeService`, `IHashIdService`, `IDateTimeService`, `ISeaFileStore`

Recommended usage:

```csharp
builder.Services.AddSeaEventDispatcher();
builder.Services.AddSeaMapper();
builder.Services.AddEfUnitOfWork<AppDbContext>();
builder.Services.AddHttpErrorLoggerMessageHandler();
builder.Services.AddSeaFileStore(cfg => { /* MinIO settings */ });

app.UseSeaEventDispatcher();
app.UseSeaMapper();
```

See also: [`Qualifiers/00-RestApi`](#qualifiers-samples)

### 01-Messaging.EntityFramework

- Bootstrapping: `MessagingEntityFramworkBootstrapper.AddSeaMessaging<TDbContext>(Action<SeaMessagingConfig>)`
- Configuration: `SeaMessagingConfig`
  - `PublishOutboxInstantly`, `OutboxPollingInterval`, `InboxRetentionInterval`, `InboxMessageTtl`, `UseDefaultBroker()`
- EF integration
  - `ISeaMessagingDbContext` to be implemented by your `DbContext`
  - `UseSeaMessaging` (internal) adds `OutboxSavedChangesInterceptor`
- Services
  - `ISeaPublisher` (stages or directly transports messages)
  - Background services: `OutboxPollingPublisher`, `OutboxMessageTransporter`, `InboxCleaner`
- Message consumption
  - Implement `ISeaConsumer<TMessage>`; consumers are auto-discovered
  - Interface-based contracts are materialized at runtime and mapped from payloads

Minimal setup:

```csharp
builder.Services.AddSeaMessaging<AppDbContext>(cfg =>
{
    // cfg.UseDefaultBroker();
    cfg.OutboxPollingInterval = TimeSpan.FromSeconds(2);
});
```

Domain to integration flow example: see [Messaging (EF Outbox/Inbox)](#messaging-ef-outboxinbox)

### 02-Messaging.EntityFramework.Kafka

- Configure via `KafkaConfig` (`ClientId`, `ConnectionString`, `TopicName`, `ConsumingTopicNames`, `Partitions`, `ConsumerGroupId`, `ConcurrentConsumers`)
- Supports partitioned publishing and multiple concurrent consumers

```csharp
services.Configure<KafkaConfig>(cfg =>
{
    cfg.ClientId = "my-app";
    cfg.ConnectionString = "PLAINTEXT://localhost:9092";
    cfg.TopicName = "outbox";
    cfg.ConsumingTopicNames = new() { "outbox" };
    cfg.Partitions = 10;
    cfg.ConsumerGroupId = "my-app-group";
    cfg.ConcurrentConsumers = 2;
});
```

### 03-Messaging.EntityFramework.RabbitMq

- Configure via `RabbitMqConfig` (`ClientId`, `ConnectionString`)

```csharp
services.Configure<RabbitMqConfig>(cfg =>
{
    cfg.ClientId = "my-app";
    cfg.ConnectionString = "amqp://user:pass@localhost:5672";
});
```

### 04-Mediator

- Bootstrapping: `SeaMediatorBootstrapper.AddSeaMediator()`
- Contracts: `ISeaRequest`, `ISeaRequest<TResponse>`
- Handlers: `ISeaRequestHandler<TRequest>`, `ISeaRequestHandler<TRequest,TResponse>`
- Operations: `Send`, `Send<TReq,TRes>`, `Publish`

Notes:

- `Send<TReq>` resolves a single handler; `Publish<TReq>` invokes all matching handlers (fan‑out)
- Auto‑registration scans loaded assemblies for handler implementations

### Qualifiers (Samples)

- `Qualifiers/00-RestApi`: end‑to‑end wiring of controllers, mediator, events, mapper, health checks, logging, file store, caching
- `Qualifiers/01-MessagingEfKafka`: EF messaging with Kafka; check `Program.cs` for DI wiring

## Design Notes

- Outbox pattern is implemented using EF Core interceptor to ensure message persistence within the application transaction
- Consumers are interface‑based to allow schema stability and dynamic contract types at runtime
- Mediator is intentionally minimal and DI‑backed for simplicity and performance

## Troubleshooting

- Messages not leaving the DB
  - Ensure background services are running: `OutboxPollingPublisher`, `OutboxMessageTransporter`
  - If using `PublishOutboxInstantly=false`, verify `OutboxPollingInterval` and DB connectivity
  - Confirm your `DbContext` implements `ISeaMessagingDbContext`
- Consumer not invoked
  - Verify a single `ISeaConsumer<T>` per message type exists (only one is used per contract)
  - Ensure the message payload type matches the consumer’s generic interface
  - Confirm the assembly containing the consumer is loaded at runtime
- `InvalidOperationException: No service for type ...`
  - Make sure you called the relevant bootstrappers (`AddSeaMediator`, `AddSeaEventDispatcher`, `AddSeaMessaging<TDbContext>`, etc.)

## FAQ

- Why interface‑based message contracts?
  - Interfaces allow generating lightweight runtime types, keeping payload shape stable and independent from concrete classes.
- Can I override messaging table names?
  - Yes. Use EF Fluent API in your `OnModelCreating` for `OutboxMessage`, `InboxMessage`, and `ConsumedFaultMessage`.
- How do I ensure idempotency?
  - Inbox storage with TTL/retention tracks processed messages; design consumers to be idempotent.
- Can I use the mediator without messaging?
  - Yes. All packages are decoupled; reference only what you need.

- Middlewares: exception handler, `WebErrorHandlerMiddleware`
- API response helpers: `ApiActionResult`, `ApiError`, `ApiResult`
- Health checks: `AddCurlHealthCheck` for external endpoints
- HTTP: `HttpErrorLoggerMessageHandler`
- Mapping: `AddSeaMapper` + `SeaTypeAdapterConfig` shortcuts
- Services: `AddSnowflakeService`, `AddHashIdService`, `AddDateTimeService`
- File store (MinIO): `AddSeaFileStore` + health check integration
- Caching & SLO: `AddSeaRedisHybridCache`, `AddSeaIdentityProviderSlo` (see `Qualifiers/00-RestApi`)

### API responses

Wrap controller responses into a unified envelope using `ApiActionResult`:

```csharp
[ApiController]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult Get(long id)
    {
        var dto = new { id, email = "a@b.com" };
        return dto.ToActionResult(); // 200 with { result: {...} }
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateUserRequest rq)
    {
        var resource = new { id = 1L };
        return resource.ToActionResult($"/users/{resource.id}"); // 201 Created
    }
}
```

### Error handling middleware

Add the unified error pipeline for REST APIs:

```csharp
app.UseMiddleware<WebErrorHandlerMiddleware>(); // or app.UseExecptionHandler();
```

Throw `SeaException` with codes/trace ids when needed; other exceptions are normalized with consistent structure by the middleware.

### Health checks and cURL checks

```csharp
builder.Services.AddHealthChecks()
    .AddCurlHealthCheck([
        new HealthCheckEndpoint { Name = "google", Url = "https://google.com" }
    ]);

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});
```

### HttpClient error logging

Register the logging handler and add it to your clients:

```csharp
builder.Services.AddHttpErrorLoggerMessageHandler();

builder.Services.AddHttpClient("backend")
    .AddHttpMessageHandler<HttpErrorLoggerMessageHandler>();

var client = httpClientFactory.CreateClient("backend");
```

### Mapster-based mapping

Register Sea mapper and customize mappings via `SeaTypeAdapterConfig`:

```csharp
builder.Services.AddSeaMapper(cfg =>
{
    cfg.NewConfig<User, UserDto>()
       .Map(d => d.Id, s => s.Id)
       .Map(d => d.Email, s => s.Email);
});

app.UseSeaMapper(); // adds default local date/time conversions
```

### Snowflake and HashId services

```csharp
builder.Services.AddSnowflakeService(options =>
{
    options.GeneratorId = 7;
    options.Epoch = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    options.IdStructure = (43, 6, 14);
});

builder.Services.AddHashIdService(options =>
{
    options.MinHashLength = 12;
});

public sealed class IdService
{
    private readonly ISnowflakeService _snowflake;
    private readonly IHashIdService _hash;

    public long NewId() => _snowflake.CreateId();
    public string Encode(long id) => _hash.Encode(id);
    public long Decode(string hash) => _hash.Decode(hash);
}
```

### Unit of Work with EF Core

```csharp
builder.Services.AddEfUnitOfWork<AppDbContext>();

public sealed class OrderService
{
    private readonly IEfUnitOfWork _uow;

    public OrderService(IEfUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task PlaceOrder(CancellationToken ct)
    {
        await _uow.WithExecutionStrategy(async token =>
        {
            await _uow.BeginTransaction(IsolationLevel.ReadCommitted, token);
            try
            {
                // ... mutate aggregates, add entities
                await _uow.SaveChanges(token);
                await _uow.CommitTransaction(token);
            }
            catch
            {
                await _uow.RollbackTransaction(token);
                throw;
            }
        }, ct);
    }
}
```

### File store (MinIO)

```csharp
builder.Services.AddSeaFileStore(cfg =>
{
    cfg.BaseUrl = "http://localhost:9000";
    cfg.UserName = "ROOTUSER";
    cfg.Password = "CHANGEME123";
    cfg.RootName = "users";
});

public sealed class AvatarService
{
    private readonly ISeaFileStore _files;

    public AvatarService(ISeaFileStore files) => _files = files;

    public async Task<string> Upload(byte[] content, CancellationToken ct)
        => await _files.SaveFile("avatars/u1.png", "image/png", content, ct);

    public Task<string> GetPutUrl(string key) => _files.ConstructPutPresignedUrl(key, TimeSpan.FromMinutes(5));
    public Task<string> GetGetUrl(string key) => _files.ConstructGetPresignedUrl(key, TimeSpan.FromMinutes(5));
}
```

---

## Build & pack

- Build: `dotnet build` or open `SeaFramework.sln` in Visual Studio
- Pack: `dotnet pack` (per project) or run `pack.bat` to produce local NuGet packages

---

## Samples

- `Qualifiers/00-RestApi`: integrates mediator, events, mapper, health checks, logging, file store, cache
- `Qualifiers/01-MessagingEfKafka`: EF messaging with Kafka

---

## Contributing

Contributions are welcome. Please keep changes focused, follow existing style, update XML docs where appropriate, and include/adjust a qualifier sample if you change public APIs.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.


