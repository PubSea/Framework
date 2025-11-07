## PubSea Framework

### Table of Contents

- [Overview](#overview)
- [Solution Layout](#solution-layout)
  - [`00-Framework` - Core DDD and Web Utilities](#00-framework---core-ddd-and-web-utilities)
  - [`01-Messaging.EntityFramework` - EF Core Outbox/Inbox Messaging](#01-messagingentityframework---ef-core-outboxinbox-messaging)
  - [`02-Messaging.EntityFramework.Kafka` - Kafka Broker Provider](#02-messagingentityframeworkkafka---kafka-broker-provider)
  - [`03-Messaging.EntityFramework.RabbitMq` - RabbitMQ Broker Provider](#03-messagingentityframeworkrabbitmq---rabbitmq-broker-provider)
  - [`04-Mediator` - Lightweight Mediator Pattern](#04-mediator---lightweight-mediator-pattern)
  - [`Qualifiers` - Sample Projects](#qualifiers---sample-projects)
- [Package Overviews (What to install and why)](#package-overviews-what-to-install-and-why)
  - [`PubSea.Framework` (00-Framework)](#pubseaframework-00-framework)
  - [`PubSea.Mediator` (04-Mediator)](#pubseamediator-04-mediator)
  - [`PubSea.Messaging.EntityFramework` (01-Messaging.EntityFramework)](#pubseamessagingentityframework-01-messagingentityframework)
  - [`PubSea.Messaging.EntityFramework.Kafka` (02-Messaging.EntityFramework.Kafka)](#pubseamessagingentityframeworkkafka-02-messagingentityframeworkkafka)
  - [`PubSea.Messaging.EntityFramework.RabbitMq` (03-Messaging.EntityFramework.RabbitMq)](#pubseamessagingentityframeworkrabbitmq-03-messagingentityframeworkrabbitmq)
- [NuGet Packages and Features](#nuget-packages-and-features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation Options](#installation-options)
  - [Quick Start Guide](#quick-start-guide)
  - [Building from Source](#building-from-source)
  - [Minimal wiring (Web API)](#minimal-wiring-web-api)
  - [Mediator](#mediator)
  - [Domain Events](#domain-events)
  - [Messaging (EF Outbox/Inbox)](#messaging-ef-outboxinbox)
  - [Kafka Provider](#kafka-provider)
  - [RabbitMQ Provider](#rabbitmq-provider)
- [Additional Utilities (00-Framework)](#additional-utilities-00-framework)
  - [API Responses](#api-responses)
  - [Error Handling Middleware](#error-handling-middleware)
  - [Health Checks and cURL Checks](#health-checks-and-curl-checks)
  - [HttpClient Error Logging](#httpclient-error-logging)
  - [Mapster-based Mapping](#mapster-based-mapping)
  - [Snowflake and HashId Services](#snowflake-and-hashid-services)
  - [Unit of Work with EF Core](#unit-of-work-with-ef-core)
  - [File Store (MinIO)](#file-store-minio)
- [Package Reference](#package-reference)
  - [00-Framework](#00-framework)
  - [01-Messaging.EntityFramework](#01-messagingentityframework)
  - [02-Messaging.EntityFramework.Kafka](#02-messagingentityframeworkkafka)
  - [03-Messaging.EntityFramework.RabbitMq](#03-messagingentityframeworkrabbitmq)
  - [04-Mediator](#04-mediator)
  - [Qualifiers (Samples)](#qualifiers-samples)
- [Complete Service Example](#complete-service-example)
  - [Example: User Management Service](#example-user-management-service)
    - [1. Domain Models](#1-domain-models)
    - [2. DTOs](#2-dtos)
    - [3. Domain Events](#3-domain-events)
    - [4. Domain Event Handlers](#4-domain-event-handlers)
    - [5. Mediator Commands and Queries](#5-mediator-commands-and-queries)
    - [6. Integration Events (Messaging)](#6-integration-events-messaging)
    - [7. Controller](#7-controller)
    - [8. DbContext](#8-dbcontext)
    - [9. Program.cs Configuration](#9-programcs-configuration)
  - [Summary](#summary)
- [Build & Pack](#build--pack)
  - [Building from Source](#building-from-source-1)
  - [Creating NuGet Packages](#creating-nuget-packages)
  - [Package Output Location](#package-output-location)
  - [Using Local Packages](#using-local-packages)
  - [Version Management](#version-management)
- [Services Reference](#services-reference)
  - [Domain events (00-Framework)](#domain-events-00-framework)
  - [Unit of Work (00-Framework)](#unit-of-work-00-framework)
  - [Mapper (00-Framework)](#mapper-00-framework)
  - [API responses (00-Framework)](#api-responses-00-framework)
  - [Middlewares (00-Framework)](#middlewares-00-framework)
  - [Health checks and cURL checks (00-Framework)](#health-checks-and-curl-checks-00-framework)
  - [HttpClient error logging (00-Framework)](#httpclient-error-logging-00-framework)
  - [Snowflake and HashId (00-Framework)](#snowflake-and-hashid-00-framework)
  - [File store (00-Framework)](#file-store-00-framework)
  - [Caching & SLO (00-Framework)](#caching--slo-00-framework)
  - [Messaging core (01-Messaging.EntityFramework)](#messaging-core-01-messagingentityframework)
  - [Kafka provider (02-Messaging.EntityFramework.Kafka)](#kafka-provider-02-messagingentityframeworkkafka)
  - [RabbitMQ provider (03-Messaging.EntityFramework.RabbitMq)](#rabbitmq-provider-03-messagingentityframeworkrabbitmq)
  - [Mediator (04-Mediator)](#mediator-04-mediator)
- [Use-case cookbook (from Qualifiers)](#use-case-cookbook-from-qualifiers)
  - [REST API: ID generation (Snowflake)](#rest-api-id-generation-snowflake)
  - [REST API: HashId encode/decode](#rest-api-hashid-encodedecode)
  - [REST API: Hybrid cache and SLO helpers](#rest-api-hybrid-cache-and-slo-helpers)
  - [REST API: File store (MinIO)](#rest-api-file-store-minio)
  - [REST API: Health checks and error handling](#rest-api-health-checks-and-error-handling)
  - [Mediator: Commands, queries, and publish](#mediator-commands-queries-and-publish)
  - [Domain Events: Publishing internal events](#domain-events-publishing-internal-events)
  - [Messaging with EF + Kafka (Outbox/Inbox)](#messaging-with-ef--kafka-outboxinbox)
- [Design Notes](#design-notes)
- [Troubleshooting](#troubleshooting)
- [FAQ](#faq)
- [Samples](#samples)
- [Contributing](#contributing)
- [License](#license)

### Overview

**What is PubSea Framework?**

SeaFramework is an opinionated ASP.NET Core framework that applies Domain-Driven Design (DDD) patterns and provides production-ready utilities: a lightweight Mediator, an Entity Framework–backed Outbox/Inbox messaging module with pluggable brokers (Kafka, RabbitMQ, or a no‑op default), mapping helpers, health checks, error handling middleware, Snowflake/HashId services, file storage (MinIO), hybrid caching, and more.

**Why use PubSea Framework?**

- **DDD Patterns**: Built-in support for Domain-Driven Design principles including aggregates, domain events, and unit of work
- **Production-Ready**: Battle-tested utilities for error handling, health checks, logging, and observability
- **Modular Architecture**: Use packages together for a full stack, or reference only what you need
- **Messaging Reliability**: Implements the Outbox/Inbox pattern to ensure message delivery and prevent data inconsistencies
- **Broker Agnostic**: Switch between Kafka, RabbitMQ, or no-op broker without changing your business logic
- **Auto-Discovery**: Handlers, consumers, and event handlers are automatically discovered and registered
- **Performance**: Lightweight mediator and efficient background services for high-throughput scenarios

**When to use PubSea Framework?**

- Building microservices or modular monoliths with ASP.NET Core
- Need reliable messaging between services (Outbox/Inbox pattern)
- Want to follow Domain-Driven Design principles
- Require production-ready cross-cutting concerns (error handling, health checks, logging)
- Need scalable ID generation (Snowflake) and user-friendly IDs (HashId)

### Solution Layout

This section explains the organization of the solution and what each project provides.

#### `00-Framework` - Core DDD and Web Utilities

**Purpose**: The foundation package containing all core Domain-Driven Design building blocks and web utilities.

**Key Components**:
- **Domain Events and Dispatcher** (`IEvent`, `IEventHandler<T>`, `AddSeaEventDispatcher`): In-process event publishing system for decoupling domain logic. Events are dispatched synchronously within the same process, making them ideal for domain events that don't need external messaging.
- **Unit of Work** (`IUnitOfWork`, `IEfUnitOfWork`, `AddEfUnitOfWork<TDbContext>`): Manages transactional boundaries and ensures atomicity across multiple operations. Provides execution strategies for resilience.
- **Middlewares** (exception handler, web error handler): Global error handling that normalizes exceptions into consistent API responses with trace IDs for debugging.
- **API Response Helpers**: Unified response envelope (`ApiActionResult`, `ApiResult`, `ApiError`) ensuring consistent API contracts across all endpoints.
- **Health Checks** (cURL checks): Synthetic health checks for external dependencies using cURL commands, useful for monitoring upstream services.
- **HTTP Error Logging Handler**: Automatic logging of HTTP client failures with cURL-reproducible request logs.
- **Mapster-based Mapper** (`AddSeaMapper`, `UseSeaMapper`): Type-safe mapping configuration and execution, reducing boilerplate mapping code.
- **Snowflake/HashId/DateTime Services**: 
  - Snowflake: Time-ordered unique ID generation (Twitter Snowflake algorithm)
  - HashId: Encoding/decoding of numeric IDs into short, URL-safe strings
  - DateTime: Persian/Jalali calendar conversions and timezone handling
- **File Storage (MinIO)** (`AddSeaFileStore`): S3-compatible file storage with presigned URLs for secure file access.
- **Utilities**: Caching extensions (Redis hybrid cache), SLO (Single Logout) helpers, logging utilities, and common helpers.

#### `01-Messaging.EntityFramework` - EF Core Outbox/Inbox Messaging

**Purpose**: Implements the Outbox/Inbox pattern using Entity Framework Core for reliable, transactional messaging.

**Key Components**:
- **Outbox Model and Interceptor**: EF Core interceptor that automatically captures published messages and stores them in the database within the same transaction as your business data.
- **Background Services**:
  - `OutboxPollingPublisher`: Polls the database for unpublished outbox messages and enqueues them for transport
  - `OutboxMessageTransporter`: Transports outbox messages to the configured message broker (Kafka/RabbitMQ)
  - `InboxCleaner`: Manages retention and TTL of processed inbox messages for idempotency tracking
- **Auto-Discovery**: Automatically discovers and registers `ISeaConsumer<TMessage>` implementations from loaded assemblies
- **Interface-Based Contracts**: Message contracts are defined as interfaces, allowing dynamic type materialization and schema stability across service boundaries

**Why Outbox/Inbox?**
- Ensures messages are published only after business data is committed (transactional consistency)
- Prevents message loss if the broker is temporarily unavailable
- Enables idempotent message consumption via inbox tracking
- Decouples business logic from message broker availability

#### `02-Messaging.EntityFramework.Kafka` - Kafka Broker Provider

**Purpose**: Provides Kafka-specific implementation for the messaging module.

**Features**:
- Partitioned message publishing for horizontal scalability
- Consumer group support for load balancing
- Concurrent consumer support for throughput optimization
- Topic-based message routing

#### `03-Messaging.EntityFramework.RabbitMq` - RabbitMQ Broker Provider

**Purpose**: Provides RabbitMQ-specific implementation for the messaging module.

**Features**:
- Exchange and queue-based routing
- Simple broker setup with broad ecosystem support
- Suitable for smaller-scale deployments or when Kafka is overkill

#### `04-Mediator` - Lightweight Mediator Pattern

**Purpose**: Implements the Mediator pattern for in-process request/command/query handling.

**Features**:
- `Send` for single handler resolution (commands/queries)
- `Publish` for fan-out to multiple handlers (events)
- Automatic handler discovery from loaded assemblies
- Dependency injection-based handler resolution
- No reflection overhead at runtime

**Benefits**:
- Decouples controllers from business logic
- Enables CQRS (Command Query Responsibility Segregation) patterns
- Simplifies testing by allowing handler isolation
- Reduces coupling between components

#### `Qualifiers` - Sample Projects

- **`Qualifiers/00-RestApi`**: Complete runnable sample demonstrating all framework features in a REST API context
- **`Qualifiers/01-MessagingEfKafka`**: End-to-end example of messaging with Kafka, including producers and consumers

### Package Overviews (What to install and why)

This section helps you decide which packages to install based on your needs.

#### `PubSea.Framework` (00-Framework)

**Install when**: You need DDD building blocks, web utilities, or any cross-cutting concerns.

**Provides**:
- DDD primitives: domain events/dispatcher, unit of work, aggregate root base classes
- Web utilities: middlewares, API response helpers, health checks, HTTP error logging
- Services: mapper (Mapster), Snowflake/HashId/DateTime services, file store (MinIO)
- Utilities: caching extensions, SLO helpers, logging helpers

**Use cases**:
- Building REST APIs or web services
- Need error handling, health checks, or logging
- Want domain events for in-process communication
- Require file storage, ID generation, or mapping utilities
- Shared cross-cutting infrastructure across multiple projects

**Dependencies**: None (foundation package)

#### `PubSea.Mediator` (04-Mediator)

**Install when**: You want to implement CQRS or decouple controllers from business logic.

**Provides**:
- Mediator pattern implementation
- Automatic handler discovery and registration
- `Send` for commands/queries, `Publish` for events
- Dependency injection integration

**Use cases**:
- Implementing CQRS (Command Query Responsibility Segregation)
- Decoupling API controllers from business logic
- Simplifying request handling pipeline
- Enabling handler-based testing

**Dependencies**: None (standalone)

#### `PubSea.Messaging.EntityFramework` (01-Messaging.EntityFramework)

**Install when**: You need reliable messaging between services using the Outbox/Inbox pattern.

**Provides**:
- Outbox/Inbox pattern implementation using EF Core
- Broker-agnostic message publishing (`ISeaPublisher`)
- Automatic consumer discovery
- Background services for message transport and cleanup
- EF Core interceptor for transactional message persistence

**Use cases**:
- Microservices communication
- Event-driven architecture
- Reliable message delivery requirements
- Need transactional message publishing

**Dependencies**: `PubSea.Framework`

#### `PubSea.Messaging.EntityFramework.Kafka` (02-Messaging.EntityFramework.Kafka)

**Install when**: You use Kafka as your message broker and need high throughput/partitioning.

**Provides**:
- Kafka transport provider
- Partitioned publishing support
- Consumer group and concurrent consumer support
- Topic-based routing

**Use cases**:
- High-throughput messaging scenarios
- Need horizontal scalability via partitioning
- Large-scale microservices architectures
- Event streaming requirements

**Dependencies**: `PubSea.Messaging.EntityFramework`

#### `PubSea.Messaging.EntityFramework.RabbitMq` (03-Messaging.EntityFramework.RabbitMq)

**Install when**: You use RabbitMQ as your message broker and prefer simpler setup.

**Provides**:
- RabbitMQ transport provider
- Exchange and queue-based routing
- Simple broker configuration

**Use cases**:
- Smaller-scale messaging needs
- Prefer RabbitMQ's simpler model over Kafka
- Existing RabbitMQ infrastructure
- Work queue patterns

**Dependencies**: `PubSea.Messaging.EntityFramework`

**Note**: You only need ONE broker provider (`Kafka` OR `RabbitMq`), not both.

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

## Getting Started

This section guides you through setting up and using PubSea framework in your ASP.NET Core application.

### Prerequisites

- .NET 9.0 or later
- ASP.NET Core Web API project
- Visual Studio 2022 or VS Code (optional, can use CLI)

### Installation Options

1. **Reference from Solution** (Development): Open `SeaFramework.sln` and reference projects directly from your solution
2. **NuGet Packages** (Production): Install packages from NuGet or local feed:
   ```bash
   dotnet pack  # Build packages locally
   # Or use pack.bat on Windows
   ```

### Quick Start Guide

1. Install the packages you need (see [Package Overviews](#package-overviews-what-to-install-and-why))
2. Configure services in `Program.cs` (see [Minimal wiring](#minimal-wiring-web-api))
3. Implement your handlers, consumers, or event handlers
4. Use framework features in your controllers or services

### Building from Source

Open the solution `SeaFramework.sln` in Visual Studio (or run `dotnet build`). You can reference projects directly or pack them as NuGet packages with `dotnet pack` (or `pack.bat`) and consume them from a local feed.

### Minimal wiring (Web API)

**Explanation**: This is the minimal setup to get started with PubSea framework. It configures the core services needed for a basic ASP.NET Core Web API application.

**What each line does**:
- `AddSeaMediator()`: Registers the mediator pattern implementation and auto-discovers all request/command/query handlers
- `AddSeaEventDispatcher()`: Registers the domain event dispatcher and auto-discovers all event handlers (`IEventHandler<T>`)
- `AddSeaMapper()`: Configures Mapster for object-to-object mapping with sensible defaults
- `AddSnowflakeService()`, `AddHashIdService()`, `AddDateTimeService()`: Optional utility services for ID generation and date/time handling
- `UseSeaMapper()`: Adds middleware for automatic mapping configuration (e.g., date/time conversions)
- `UseSeaEventDispatcher()`: Initializes the event dispatcher middleware for in-process event handling

**When to use**: Start here for a basic REST API that uses mediator pattern, domain events, and mapping utilities.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Mediator - enables CQRS pattern with automatic handler discovery
builder.Services.AddSeaMediator();

// Domain events - in-process event publishing (auto-registers IEventHandler<T>)
builder.Services.AddSeaEventDispatcher();

// Mapper - Mapster-based object mapping with sensible defaults
builder.Services.AddSeaMapper();

// Optional utilities - ID generation and date/time services
builder.Services.AddSnowflakeService();      // Time-ordered unique IDs
builder.Services.AddHashIdService();         // Hash-based ID encoding
builder.Services.AddDateTimeService();       // Persian calendar conversions

var app = builder.Build();

// Middleware configuration
app.UseSeaMapper();           // Enables mapping features
app.UseSeaEventDispatcher();  // Enables domain event dispatching

app.MapControllers();
app.Run();
```

### Mediator

**Explanation**: The Mediator pattern decouples the sender (controller) from the receiver (handler), promoting single responsibility and making your code more testable. PubSea framework's mediator automatically discovers and registers handlers from all loaded assemblies.

**Key Concepts**:
- **Command**: `ISeaRequest` without return type - represents an operation that changes state (e.g., `CreateUser`)
- **Query**: `ISeaRequest<TResponse>` with return type - represents a read operation (e.g., `GetUser`)
- **Send**: Dispatches to a single handler (use for commands and queries)
- **Publish**: Dispatches to all matching handlers (use for events/fan-out)

**Benefits**:
- Controllers don't need to know about business logic implementation
- Handlers can be easily unit tested in isolation
- Enables CQRS pattern (separate read/write models)
- Reduces coupling between layers

**Example - Command (Write Operation)**:

```csharp
// Command definition (no return type)
public sealed record CreateUser(string Email) : ISeaRequest;

// Command handler (implements ISeaRequestHandler<TRequest>)
public sealed class CreateUserHandler : ISeaRequestHandler<CreateUser>
{
    private readonly AppDbContext _db;
    
    public CreateUserHandler(AppDbContext db) => _db = db;
    
    public async Task Handle(CreateUser request, CancellationToken ct = default)
    {
        var user = new User { Email = request.Email };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }
}
```

**Example - Query (Read Operation)**:

```csharp
// Query definition (with return type)
public sealed record GetUser(long Id) : ISeaRequest<UserDto>;

// Query handler (implements ISeaRequestHandler<TRequest, TResponse>)
public sealed class GetUserHandler : ISeaRequestHandler<GetUser, UserDto>
{
    private readonly AppDbContext _db;
    
    public GetUserHandler(AppDbContext db) => _db = db;
    
    public async Task<UserDto> Handle(GetUser request, CancellationToken ct = default)
    {
        var user = await _db.Users.FindAsync([request.Id], ct);
        return user is null 
            ? throw new NotFoundException($"User {request.Id} not found")
            : new UserDto { Id = user.Id, Email = user.Email };
    }
}
```

**Usage in Controllers**:

```csharp
[ApiController]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    private readonly ISeaMediator _mediator;
    
    public UsersController(ISeaMediator mediator) => _mediator = mediator;
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req, CancellationToken ct)
    {
        await _mediator.Send(new CreateUser(req.Email), ct);
        return Ok();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
    {
        var dto = await _mediator.Send<GetUser, UserDto>(new GetUser(id), ct);
        return Ok(dto);
    }
}
```

**Publish for Fan-Out**:

```csharp
// Publish dispatches to ALL handlers that implement ISeaRequestHandler<TRequest>
// Useful for events where multiple handlers need to react
await _mediator.Publish(new UserCreated("user@example.com"), ct);
```

**Note**: Handlers are auto-registered from loaded assemblies by `AddSeaMediator()`. No manual registration needed!

### Domain Events

**Explanation**: Domain events represent something important that happened in your domain (e.g., "UserCreated", "OrderPlaced"). They allow you to decouple domain logic by letting multiple handlers react to the same event without the domain knowing about them.

**Key Differences from Mediator**:
- **Domain Events**: In-process, synchronous event handling for domain logic. Events represent "something happened" in your domain.
- **Mediator**: Request/response pattern for commands and queries. Can be synchronous or asynchronous.
- **Integration Events (Messaging)**: Cross-service communication via message broker. Use for microservices.

**When to Use Domain Events**:
- Aggregate lifecycle events (created, updated, deleted)
- Business rule triggers (e.g., when order total exceeds $1000, send notification)
- Cross-aggregate communication within the same bounded context
- Side effects that shouldn't be part of the main transaction (logged separately)

**Example - Domain Event and Handlers**:

```csharp
// 1. Define the domain event
public sealed class UserCreated : IEnrichedDomainEvent
{
    public long UserId { get; init; }
    public string Email { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    
    // Required by IEnrichedDomainEvent
    public long EventId { get; set; }
    public DateTime PublishedUtcDateTime { get; set; }
}

// 2. Implement event handlers (multiple handlers can handle the same event)
public sealed class SendWelcomeEmailHandler : IEventHandler<UserCreated>
{
    private readonly IEmailService _emailService;
    
    public SendWelcomeEmailHandler(IEmailService emailService) 
        => _emailService = emailService;
    
    public async Task Handle(UserCreated evt, CancellationToken ct = default)
    {
        await _emailService.SendWelcomeEmail(evt.Email, ct);
    }
}

public sealed class UpdateUserStatisticsHandler : IEventHandler<UserCreated>
{
    private readonly IStatisticsService _stats;
    
    public UpdateUserStatisticsHandler(IStatisticsService stats) => _stats = stats;
    
    public async Task Handle(UserCreated evt, CancellationToken ct = default)
    {
        await _stats.IncrementUserCount(ct);
    }
}

// 3. Publish the event from your domain service or aggregate
public sealed class UserService
{
    private readonly IEventDispatcher _eventDispatcher;
    private readonly AppDbContext _db;
    
    public UserService(IEventDispatcher eventDispatcher, AppDbContext db)
    {
        _eventDispatcher = eventDispatcher;
        _db = db;
    }
    
    public async Task CreateUser(string email, CancellationToken ct)
    {
        var user = new User { Email = email, CreatedAt = DateTime.UtcNow };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        
        // Publish domain event (all handlers will be invoked)
        var evt = new UserCreated 
        { 
            UserId = user.Id, 
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
        SeaEventHelper.InitializeEvents(evt); // Sets EventId and PublishedUtcDateTime
        await _eventDispatcher.Dispatch([evt], ct);
    }
}
```

**Setup**:

```csharp
builder.Services.AddSeaEventDispatcher();  // Registers dispatcher and auto-discovers handlers
app.UseSeaEventDispatcher();               // Initializes event dispatching middleware
```

**Note**: Handlers are discovered and registered automatically. Just implement `IEventHandler<TEvent>` anywhere in your assemblies!

### Messaging (EF Outbox/Inbox)

**Explanation**: The Outbox/Inbox pattern ensures reliable message delivery in distributed systems. Messages are stored in the database within the same transaction as your business data, then asynchronously published to a message broker. This guarantees that messages are only published if the transaction succeeds.

**Key Concepts**:
- **Outbox**: Messages to be published are stored in the database before being sent to the broker
- **Inbox**: Received messages are stored before processing to ensure idempotency
- **Transactional Guarantee**: Messages are persisted in the same transaction as business data
- **Background Services**: Poll and transport messages asynchronously to/from the broker
- **Idempotency**: Inbox tracking prevents duplicate message processing

**When to Use**:
- Microservices communication
- Event-driven architecture
- Need guaranteed message delivery
- Want to decouple from broker availability

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

## Complete Service Example

This section provides a complete, production-ready example of a User Management Service that demonstrates multiple PubSea framework features working together. This example shows how to build a real-world service using Mediator, Domain Events, Unit of Work, API Responses, Mapping, and Messaging.

### Example: User Management Service

This example demonstrates:
- **Mediator Pattern**: Commands and queries for user operations
- **Domain Events**: Publishing and handling domain events
- **Unit of Work**: Transactional operations
- **API Responses**: Unified response format
- **Mapping**: Entity to DTO mapping
- **Messaging**: Integration events for cross-service communication
- **Snowflake ID Generation**: Unique ID generation

#### 1. Domain Models

```csharp
// Domain/User.cs
using PubSea.Framework.Entities;

namespace UserService.Domain;

public sealed class User : AggregateRoot<long>
{
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private User() { } // EF Core

    private User(string email, string firstName, string lastName)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(string email, string firstName, string lastName)
        => new(email, firstName, lastName);

    public void Update(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

#### 2. DTOs

```csharp
// DTOs/UserDto.cs
namespace UserService.DTOs;

public sealed record UserDto
{
    public long Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string FullName => $"{FirstName} {LastName}";
    public DateTime CreatedAt { get; init; }
}

// DTOs/CreateUserRequest.cs
namespace UserService.DTOs;

public sealed record CreateUserRequest
{
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
}

// DTOs/UpdateUserRequest.cs
namespace UserService.DTOs;

public sealed record UpdateUserRequest
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
}
```

#### 3. Domain Events

```csharp
// Domain/Events/UserCreatedEvent.cs
using PubSea.Framework.Events;

namespace UserService.Domain.Events;

public sealed class UserCreatedEvent : IEnrichedDomainEvent
{
    public long UserId { get; init; }
    public string Email { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public DateTime CreatedAt { get; init; }

    // Required by IEnrichedDomainEvent
    public long EventId { get; set; }
    public DateTime PublishedUtcDateTime { get; set; }
}

// Domain/Events/UserUpdatedEvent.cs
using PubSea.Framework.Events;

namespace UserService.Domain.Events;

public sealed class UserUpdatedEvent : IEnrichedDomainEvent
{
    public long UserId { get; init; }
    public string Email { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public DateTime UpdatedAt { get; init; }

    public long EventId { get; set; }
    public DateTime PublishedUtcDateTime { get; set; }
}
```

#### 4. Domain Event Handlers

```csharp
// Handlers/Events/UserCreatedEventHandler.cs
using PubSea.Framework.Events;
using UserService.Domain.Events;

namespace UserService.Handlers.Events;

public sealed class SendWelcomeEmailHandler : IEventHandler<UserCreatedEvent>
{
    private readonly ILogger<SendWelcomeEmailHandler> _logger;

    public SendWelcomeEmailHandler(ILogger<SendWelcomeEmailHandler> logger)
        => _logger = logger;

    public Task Handle(UserCreatedEvent evt, CancellationToken ct = default)
    {
        _logger.LogInformation("Sending welcome email to {Email} for user {UserId}", 
            evt.Email, evt.UserId);
        // TODO: Implement email sending
        return Task.CompletedTask;
    }
}

public sealed class LogUserCreationHandler : IEventHandler<UserCreatedEvent>
{
    private readonly ILogger<LogUserCreationHandler> _logger;

    public LogUserCreationHandler(ILogger<LogUserCreationHandler> logger)
        => _logger = logger;

    public Task Handle(UserCreatedEvent evt, CancellationToken ct = default)
    {
        _logger.LogInformation("User created: {UserId}, {Email}, {FullName}", 
            evt.UserId, evt.Email, evt.FullName);
        return Task.CompletedTask;
    }
}
```

#### 5. Mediator Commands and Queries

```csharp
// Commands/CreateUserCommand.cs
using PubSea.Mediator.Abstractions;

namespace UserService.Commands;

public sealed record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName
) : ISeaRequest<long>;

// Commands/CreateUserCommandHandler.cs
using PubSea.Framework.Data;
using PubSea.Framework.Events;
using PubSea.Framework.Utility;
using UserService.Data;
using UserService.Domain;
using UserService.Domain.Events;

namespace UserService.Commands;

public sealed class CreateUserCommandHandler : ISeaRequestHandler<CreateUserCommand, long>
{
    private readonly IEfUnitOfWork _uow;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ISnowflakeService _snowflake;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IEfUnitOfWork uow,
        IEventDispatcher eventDispatcher,
        ISnowflakeService snowflake,
        ILogger<CreateUserCommandHandler> logger)
    {
        _uow = uow;
        _eventDispatcher = eventDispatcher;
        _snowflake = snowflake;
        _logger = logger;
    }

    public async Task<long> Handle(CreateUserCommand request, CancellationToken ct = default)
    {
        return await _uow.WithExecutionStrategy(async token =>
        {
            await _uow.BeginTransaction(System.Data.IsolationLevel.ReadCommitted, token);
            try
            {
                var dbContext = _uow.GetDbContext<AppDbContext>();
                
                // Generate unique ID
                var userId = _snowflake.CreateId();
                
                // Create user aggregate
                var user = User.Create(request.Email, request.FirstName, request.LastName);
                user.Id = userId; // Set generated ID
                
                dbContext.Users.Add(user);
                await _uow.SaveChanges(token);

                // Publish domain event
                var evt = new UserCreatedEvent
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}",
                    CreatedAt = user.CreatedAt
                };
                SeaEventHelper.InitializeEvents(evt);
                await _eventDispatcher.Dispatch([evt], token);

                await _uow.CommitTransaction(token);
                
                _logger.LogInformation("User created successfully: {UserId}", userId);
                return userId;
            }
            catch
            {
                await _uow.RollbackTransaction(token);
                throw;
            }
        }, ct);
    }
}

// Commands/UpdateUserCommand.cs
using PubSea.Mediator.Abstractions;

namespace UserService.Commands;

public sealed record UpdateUserCommand(
    long UserId,
    string FirstName,
    string LastName
) : ISeaRequest;

// Commands/UpdateUserCommandHandler.cs
using PubSea.Framework.Data;
using PubSea.Framework.Events;
using PubSea.Framework.Exceptions;
using PubSea.Framework.Utility;
using UserService.Domain.Events;

namespace UserService.Commands;

public sealed class UpdateUserCommandHandler : ISeaRequestHandler<UpdateUserCommand>
{
    private readonly IEfUnitOfWork _uow;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IEfUnitOfWork uow,
        IEventDispatcher eventDispatcher,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _uow = uow;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken ct = default)
    {
        await _uow.WithExecutionStrategy(async token =>
        {
            await _uow.BeginTransaction(System.Data.IsolationLevel.ReadCommitted, token);
            try
            {
                var dbContext = _uow.GetDbContext<AppDbContext>();
                var user = await dbContext.Users.FindAsync([request.UserId], token);
                
                if (user == null)
                    throw new SeaException(
                        $"User {request.UserId} not found", 
                        SeaException.NOT_FOUND_CODE, 
                        ExceptionStatus.NotFound);

                user.Update(request.FirstName, request.LastName);
                await _uow.SaveChanges(token);

                // Publish domain event
                var evt = new UserUpdatedEvent
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}",
                    UpdatedAt = user.UpdatedAt!.Value
                };
                SeaEventHelper.InitializeEvents(evt);
                await _eventDispatcher.Dispatch([evt], token);

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

// Queries/GetUserQuery.cs
using PubSea.Mediator.Abstractions;
using UserService.DTOs;

namespace UserService.Queries;

public sealed record GetUserQuery(long UserId) : ISeaRequest<UserDto?>;

// Queries/GetUserQueryHandler.cs
using PubSea.Framework.Data;
using PubSea.Framework.Mapping;
using UserService.Data;
using UserService.Domain;
using UserService.DTOs;

namespace UserService.Queries;

public sealed class GetUserQueryHandler : ISeaRequestHandler<GetUserQuery, UserDto?>
{
    private readonly IEfUnitOfWork _uow;
    private readonly ISeaMapper _mapper;

    public GetUserQueryHandler(IEfUnitOfWork uow, ISeaMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetUserQuery request, CancellationToken ct = default)
    {
        var dbContext = _uow.GetDbContext<AppDbContext>();
        var user = await dbContext.Users.FindAsync([request.UserId], ct);
        
        return user == null ? null : _mapper.Map<User, UserDto>(user);
    }
}
```

#### 6. Integration Events (Messaging)

```csharp
// IntegrationEvents/IUserCreated.cs
namespace UserService.IntegrationEvents;

public interface IUserCreated
{
    long UserId { get; set; }
    string Email { get; set; }
    string FullName { get; set; }
    DateTime CreatedAt { get; set; }
}

// Consumers/UserCreatedConsumer.cs
using PubSea.Messaging.EntityFramework.Abstractions;
using UserService.IntegrationEvents;

namespace UserService.Consumers;

public sealed class UserCreatedConsumer : ISeaConsumer<IUserCreated>
{
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(ILogger<UserCreatedConsumer> logger)
        => _logger = logger;

    public Task Consume(IUserCreated message, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Received integration event: UserCreated - UserId: {UserId}, Email: {Email}", 
            message.UserId, message.Email);
        
        // TODO: Update other microservices, send notifications, etc.
        return Task.CompletedTask;
    }
}
```

#### 7. Controller

```csharp
// Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using PubSea.Framework.ApiResponse;
using PubSea.Mediator.Abstractions;
using UserService.Commands;
using UserService.DTOs;
using UserService.Queries;

namespace UserService.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ISeaMediator _mediator;

    public UsersController(ISeaMediator mediator)
        => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken ct = default)
    {
        var userId = await _mediator.Send(
            new CreateUserCommand(request.Email, request.FirstName, request.LastName),
            ct);

        return new { UserId = userId }
            .ToActionResult($"/api/users/{userId}");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id, CancellationToken ct = default)
    {
        var user = await _mediator.Send<GetUserQuery, UserDto?>(new GetUserQuery(id), ct);
        
        if (user == null)
            return NotFound();

        return user.ToActionResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdateUserRequest request,
        CancellationToken ct = default)
    {
        await _mediator.Send(
            new UpdateUserCommand(id, request.FirstName, request.LastName),
            ct);

        return NoContent();
    }
}
```

#### 8. DbContext

```csharp
// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using PubSea.Messaging.EntityFramework.EntityFramework;
using UserService.Domain;

namespace UserService.Data;

public sealed class AppDbContext : DbContext, ISeaMessagingDbContext
{
    public DbSet<User> Users { get; set; } = null!;

    // Required by ISeaMessagingDbContext
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;
    public DbSet<ConsumedFaultMessage> ConsumedFaultMessages { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}
```

#### 9. Program.cs Configuration

```csharp
// Program.cs
using Microsoft.EntityFrameworkCore;
using PubSea.Framework.Data;
using PubSea.Framework.DomainModel;
using PubSea.Framework.Middlewares;
using PubSea.Framework.Services.Abstractions;
using PubSea.Mediator;
using PubSea.Messaging.EntityFramework;
using PubSea.Messaging.EntityFramework.Kafka.Configs;
using UserService.Data;
using UserService.Domain;
using UserService.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Entity Framework
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

// Unit of Work
builder.Services.AddEfUnitOfWork<AppDbContext>();

// Mediator
builder.Services.AddSeaMediator();

// Domain Events
builder.Services.AddSeaEventDispatcher();

// Mapper
builder.Services.AddSeaMapper(config =>
{
    config.NewConfig<User, UserDto>()
        .Map(d => d.Id, s => s.Id)
        .Map(d => d.Email, s => s.Email)
        .Map(d => d.FirstName, s => s.FirstName)
        .Map(d => d.LastName, s => s.LastName)
        .Map(d => d.CreatedAt, s => s.CreatedAt);
});

// Services
builder.Services.AddSnowflakeService(options =>
{
    options.GeneratorId = 1;
    options.Epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    options.IdStructure = (41, 10, 12);
});

// Messaging (with Kafka)
builder.Services.AddSeaMessaging<AppDbContext>(config =>
{
    config.PublishOutboxInstantly = false;
    config.OutboxPollingInterval = TimeSpan.FromSeconds(5);
    config.UseKafkaBroker(kafkaConfig =>
    {
        kafkaConfig.ClientId = "user-service";
        kafkaConfig.ConnectionString = builder.Configuration.GetConnectionString("Kafka")!;
        kafkaConfig.TopicName = "user-events";
        kafkaConfig.ConsumingTopicNames = ["user-events"];
        kafkaConfig.ConsumerGroupId = "user-service-consumers";
        kafkaConfig.ConcurrentConsumers = 2;
    });
});

// Error Handling
builder.Services.AddHealthChecks();

var app = builder.Build();

// Middleware
app.UseSeaMapper();
app.UseSeaEventDispatcher();
app.UseMiddleware<WebErrorHandlerMiddleware>();

app.MapControllers();
app.Run();
```

### Summary

This complete example demonstrates:

1. **Domain-Driven Design**: Aggregate roots, domain events, and value objects
2. **Mediator Pattern**: Commands for writes, queries for reads
3. **Unit of Work**: Transactional consistency across operations
4. **Domain Events**: In-process event handling for domain logic
5. **Integration Events**: Cross-service communication via messaging
6. **API Responses**: Unified response format with proper HTTP status codes
7. **Mapping**: Type-safe DTO mapping using Mapster
8. **ID Generation**: Snowflake for unique, time-ordered IDs
9. **Error Handling**: Global exception handling middleware
10. **Messaging**: Reliable message delivery using Outbox/Inbox pattern

This service is production-ready and demonstrates best practices for building scalable, maintainable microservices with PubSea framework.

---

## Build & Pack

**Explanation**: This section explains how to build the framework from source and create NuGet packages for distribution.

### Building from Source

To build the entire solution:

```bash
dotnet build SeaFramework.sln
```

Or open `SeaFramework.sln` in Visual Studio 2022 and build from there.

### Creating NuGet Packages

Each project in the solution produces a NuGet package. To create packages:

**Option 1: Build all packages at once (Windows)**
```bash
pack.bat
```

**Option 2: Build packages individually**
```bash
cd 00-Framework
dotnet pack -c Release

cd ../01-Messaging.EntityFramework
dotnet pack -c Release

cd ../02-Messaging.EntityFramework.Kafka
dotnet pack -c Release

cd ../03-Messaging.EntityFramework.RabbitMq
dotnet pack -c Release

cd ../04-Mediator
dotnet pack -c Release
```

**Option 3: Build all packages in one command**
```bash
dotnet pack SeaFramework.sln -c Release
```

### Package Output Location

Packages are created in the `packages/` directory at the solution root. The package files (`.nupkg`) can be:

1. **Published to NuGet.org**: Use `dotnet nuget push` to publish to the public NuGet feed
2. **Added to a Local Feed**: Create a local NuGet feed for internal distribution
3. **Referenced Directly**: Reference the `.nupkg` files directly in your projects (development only)

### Using Local Packages

To use locally built packages in your projects:

**Option 1: Add local NuGet source**
```bash
dotnet nuget add source D:\Projects\Sea\SeaFramework\packages --name LocalSeaFramework
```

Then reference packages normally:
```xml
<PackageReference Include="PubSea.Framework" />
```

**Option 2: Reference directly (development)**
```xml
<ProjectReference Include="..\..\SeaFramework\00-Framework\00-Framework.csproj" />
```

### Version Management

Package versions are managed in `Directory.Build.props`. Update the version there before building packages.

---

## Services Reference

This section explains each core service/component, its purpose, usage, and benefits.

### Domain events (00-Framework)

- Interfaces: `IEvent`, `IEventHandler<TEvent>`, `IEventDispatcher`
- Bootstrapping: `AddSeaEventDispatcher()` registers the dispatcher and auto-discovers handlers; `UseSeaEventDispatcher()` initializes runtime helpers.
- Benefit: Enables decoupled, in-process event handling for aggregate lifecycle notifications without tight coupling.

Usage:

```csharp
builder.Services.AddSeaEventDispatcher();
app.UseSeaEventDispatcher();
```

### Unit of Work (00-Framework)

- Interfaces: `IUnitOfWork`, `IEfUnitOfWork`
- Implementation: `AddEfUnitOfWork<TDbContext>()` provides transactional boundaries (`BeginTransaction`, `Commit`, `Rollback`) and resilient execution strategy blocks.
- Benefit: Ensures consistent commits across multiple repository/aggregate operations and simplifies transactional orchestration.

### Mapper (00-Framework)

- Types: `SeaTypeAdapterConfig`, `ISeaMapper`
- Bootstrapping: `AddSeaMapper(Action<SeaTypeAdapterConfig>?)`, `UseSeaMapper()` adds sensible defaults.
- Benefit: Centralizes mapping configuration with Mapster, reduces repetitive mapping code.

### API responses (00-Framework)

- Types: `ApiActionResult`, `ApiResult`, `ApiError`
- Benefit: Unified response envelope and created-location responses; consistent error shape across the API.

### Middlewares (00-Framework)

- `WebErrorHandlerMiddleware`, `ExceptionMiddleware`
- Benefit: Normalizes errors, logs with trace IDs, returns consistent problem payloads. Helps SREs and clients diagnose issues.

### Health checks and cURL checks (00-Framework)

- `AddCurlHealthCheck(params HealthCheckEndpoint[])`
- Benefit: Synthetic external checks (e.g., upstream dependencies) with aggressive timeouts.

### HttpClient error logging (00-Framework)

- `HttpErrorLoggerMessageHandler`
- Benefit: Automatic cURL reproduction logging and response body capture on non-success HTTP responses.

### Snowflake and HashId (00-Framework)

- `ISnowflakeService`: time-ordered unique IDs (configurable epoch/structure)
- `IHashIdService`: hashids encoding/decoding (short public IDs)
- Benefit: High-throughput ID generation and user-friendly ID exposure.

### File store (00-Framework)

- Types: `ISeaFileStore`, `SeaFileStore`, `SeaFileStoreConfig`
- Bootstrapping: `AddSeaFileStore(Action<SeaFileStoreConfig>)`
- Benefit: Simple file persistence over MinIO with presigned URLs and zipping helpers.

### Caching & SLO (00-Framework)

- Extensions: `AddSeaRedisHybridCache(...)`, `AddSeaIdentityProviderSlo(...)`
- Benefit: Efficient multi-tier caching and session logout signaling patterns.

### Messaging core (01-Messaging.EntityFramework)

- Abstractions: `ISeaPublisher`, `ISeaConsumer<T>`, `ISeaMessagingDbContext`
- Hosted services: `OutboxPollingPublisher`, `OutboxMessageTransporter`, `InboxCleaner`
- Config: `SeaMessagingConfig` (poll intervals, TTLs, default broker)
- EF Integration: `UseSeaMessaging` adds `OutboxSavedChangesInterceptor`
- Benefit: Outbox/Inbox pattern for reliable, idempotent messaging across any EF-supported database; decouples producers from concrete brokers.

Usage sketch:

```csharp
builder.Services.AddSeaMessaging<AppDbContext>(cfg =>
{
    cfg.OutboxPollingInterval = TimeSpan.FromSeconds(2);
});
```

### Kafka provider (02-Messaging.EntityFramework.Kafka)

- Config: `KafkaConfig` (`ClientId`, `ConnectionString`, `TopicName`, `ConsumingTopicNames`, `Partitions`, `ConsumerGroupId`, `ConcurrentConsumers`)
- Benefit: Horizontal scalability with partitions, consumer groups, and concurrency knobs.

### RabbitMQ provider (03-Messaging.EntityFramework.RabbitMq)

- Config: `RabbitMqConfig` (`ClientId`, `ConnectionString`)
- Benefit: Simpler broker setup with broad ecosystem support.

### Mediator (04-Mediator)

- Interfaces: `ISeaMediator`, `ISeaRequest`, `ISeaRequest<TResponse>`, handlers for both
- Bootstrapping: `AddSeaMediator()`
- Benefit: In-process request pipeline abstraction with fan-out publish support and DI-based handler discovery.

## Use-case cookbook (from Qualifiers)

Practical snippets showing typical usage mirrored in `Qualifiers/00-RestApi` and `Qualifiers/01-MessagingEfKafka`.

### REST API: ID generation (Snowflake)

```csharp
[ApiController]
[Route("ids")]
public sealed class IdsController : ControllerBase
{
    private readonly ISnowflakeService _snowflake;
    public IdsController(ISnowflakeService snowflake) => _snowflake = snowflake;

    [HttpGet]
    public IActionResult New() => new { id = _snowflake.CreateId() }.ToActionResult();
}
```

DI setup (Program.cs):

```csharp
builder.Services.AddSnowflakeService(options =>
{
    options.GeneratorId = 255;
    options.Epoch = new DateTime(2022, 9, 18, 0, 0, 0, DateTimeKind.Utc);
    options.IdStructure = (41, 10, 12);
});
```

### REST API: HashId encode/decode

```csharp
[ApiController]
[Route("hash")] 
public sealed class HashController : ControllerBase
{
    private readonly IHashIdService _hash;
    public HashController(IHashIdService hash) => _hash = hash;

    [HttpGet("encode/{num}")]
    public IActionResult Encode(long num) => new { value = _hash.Encode(num) }.ToActionResult();

    [HttpGet("decode/{hash}")]
    public IActionResult Decode(string hash) => new { value = _hash.Decode(hash) }.ToActionResult();
}
```

DI setup:

```csharp
builder.Services.AddHashIdService(options =>
{
    options.MinHashLength = 20;
});
```

### REST API: Hybrid cache and SLO helpers

```csharp
builder.Services.AddSeaRedisHybridCache(options =>
{
    options.ConfigureRedis(r =>
    {
        r.Configuration = "localhost:6379";
        r.InstanceName = "sample__";
    });
});

builder.Services.AddSeaIdentityProviderSlo("localhost:6379");
```

### REST API: File store (MinIO)

```csharp
builder.Services.AddSeaFileStore(config =>
{
    config.BaseUrl = "http://localhost:9000";
    config.UserName = "ROOTUSER";
    config.Password = "CHANGEME123";
    config.RootName = "users";
});

[ApiController]
[Route("files")]
public sealed class FilesController : ControllerBase
{
    private readonly ISeaFileStore _files;
    public FilesController(ISeaFileStore files) => _files = files;

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);
        var key = await _files.SaveFile($"avatars/{Guid.NewGuid()}.png", file.ContentType, ms.ToArray(), ct);
        return new { key }.ToActionResult($"/files/{key}");
    }
}
```

### REST API: Health checks and error handling

```csharp
// Health checks
builder.Services.AddHealthChecks()
    .AddCurlHealthCheck([
        new HealthCheckEndpoint { Name = "google", Url = "https://google.com" }
    ]);

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

// Error handling
app.UseMiddleware<WebErrorHandlerMiddleware>();
```

### Mediator: Commands, queries, and publish

```csharp
public sealed record CreateUser(string Email) : ISeaRequest;
public sealed class CreateUserHandler : ISeaRequestHandler<CreateUser>
{
    public Task Handle(CreateUser request, CancellationToken ct = default) => Task.CompletedTask;
}

public sealed record GetUser(long Id) : ISeaRequest<UserDto>;
public sealed class GetUserHandler : ISeaRequestHandler<GetUser, UserDto>
{
    public Task<UserDto> Handle(GetUser request, CancellationToken ct = default)
        => Task.FromResult(new UserDto());
}

await mediator.Send(new CreateUser("a@b.com"), ct);
var dto = await mediator.Send<GetUser, UserDto>(new GetUser(1), ct);
await mediator.Publish(new CreateUser("c@d.com"), ct);
```

DI setup:

```csharp
builder.Services.AddSeaMediator();
```

### Domain Events: Publishing internal events

```csharp
public sealed class UserCreated : IEvent { public long Id { get; init; } }

public sealed class UserCreationHandler : IEventHandler<UserCreated>
{
    public Task Handle(UserCreated e, CancellationToken ct = default)
    {
        // react to user creation
        return Task.CompletedTask;
    }
}

builder.Services.AddSeaEventDispatcher();
app.UseSeaEventDispatcher();
```

### Messaging with EF + Kafka (Outbox/Inbox)

```csharp
// Register messaging
builder.Services.AddSeaMessaging<AppDbContext>(cfg =>
{
    // cfg.UseDefaultBroker(); // no-op provider for local/dev
    cfg.OutboxPollingInterval = TimeSpan.FromSeconds(2);
});

// Configure Kafka provider
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

// Publish events transactionally
public sealed class UserService
{
    private readonly ISeaPublisher _publisher;
    private readonly ISeaMessagingDbContext _db;

    public async Task CreateUser(CancellationToken ct)
    {
        // persist aggregate...
        await _publisher.Publish([ new UserCreated { Id = 1 } ], ct);
        await _db.SaveChangesAsync(ct);
    }
}

// Consume
public interface IUserCreated { long Id { get; set; } }
public sealed class UserCreatedConsumer : ISeaConsumer<IUserCreated>
{
    public Task Consume(IUserCreated message, CancellationToken ct = default)
        => Task.CompletedTask;
}
```

DB context requirements:

```csharp
public sealed class AppDbContext : DbContext, ISeaMessagingDbContext
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;
    public DbSet<ConsumedFaultMessage> ConsumedFaultMessages { get; set; } = null!;
}
```

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


