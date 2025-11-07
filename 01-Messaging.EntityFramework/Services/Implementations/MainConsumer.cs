using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PubSea.Framework.Data;
using PubSea.Messaging.EntityFramework.EntityFramework;
using PubSea.Messaging.EntityFramework.Models;
using PubSea.Messaging.EntityFramework.Services.Abstractions;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PubSea.Messaging.EntityFramework.Services.Implementations;

internal sealed class MainConsumer : IMainConsumer
{
    private const byte RETRY_COUNT = 5;
    private readonly ILogger<MainConsumer> _logger;
    private readonly IInboxService _inboxService;
    private readonly ISeaMessagingDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    private byte _iterationCount = 1;

    public MainConsumer(ILogger<MainConsumer> logger, IInboxService inboxService,
        ISeaMessagingDbContext dbContext, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _inboxService = inboxService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    async Task IMainConsumer.Consume(IServiceScope scope, OutboxMessage message, CancellationToken ct)
    {
        try
        {
            await DoConsuming(scope, message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong in MainConsumer. {IterationCount}", _iterationCount);

            if (_iterationCount < RETRY_COUNT)
            {
                _iterationCount++;
                await ((IMainConsumer)this).Consume(scope, message, ct);
                return;
            }

            await SaveFaultMessage(scope, message, ex, ct);
        }
    }

    private async Task DoConsuming(IServiceScope scope, OutboxMessage message, CancellationToken ct)
    {
        ClearDbContextChangeTrackerFromPreviousIteration();

        var messageAlreadyProcessed = await _inboxService.Exists(message.Id, ct);

        if (messageAlreadyProcessed)
        {
            return;
        }

        Type? payloadType = null;
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(message.Type);
            if (type is not null)
            {
                payloadType = type;
                break;
            }
        }

        if (payloadType is null)
        {
            return;
        }

        var payload = ExtractPayload(message, payloadType);

        InboxMessage inboxMessage = new()
        {
            Id = message.Id,
        };

        await _dbContext.InboxMessages.AddAsync(inboxMessage, ct);

        var result = await ConsumeByTargetConsumer(scope, payloadType, payload, ct);
        if (result.ShouldBreakContinuation)
        {
            return;
        }

        await _unitOfWork.SaveChanges(ct);
    }

    private void ClearDbContextChangeTrackerFromPreviousIteration()
    {
        ((DbContext)_dbContext).ChangeTracker.Clear();
    }

    private static object? ExtractPayload(OutboxMessage message, Type payloadType)
    {
        var realPayloadType = MessageContracts.Types!.GetValueOrDefault(payloadType.FullName);

        if (realPayloadType is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize(message.Payload, realPayloadType, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
    }

    private async Task<ConsumeResult> ConsumeByTargetConsumer(IServiceScope scope, Type payloadType, object? payload, CancellationToken ct)
    {
        var consumerMainType = typeof(ISeaConsumer<>);
        Type[] typeArgs = [payloadType!,];
        var consumerType = consumerMainType.MakeGenericType(typeArgs);

        object? consumer = null;
        try
        {
            consumer = scope.ServiceProvider.GetRequiredService(consumerType);
        }
        catch (InvalidOperationException)
        {
            return ConsumeResult.Break;
        }

        var task = consumerType
            .GetMethod(nameof(ISeaConsumer<object>.Consume))
            !.Invoke(consumer, [payload, ct]);

        await (Task)task!;
        return ConsumeResult.NotBreak;
    }

    private async Task SaveFaultMessage(IServiceScope scope, OutboxMessage message, Exception ex, CancellationToken ct)
    {
        ClearDbContextChangeTrackerFromPreviousIteration();
        var faultService = scope.ServiceProvider.GetRequiredService<IConsumedFaultService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var error = new Dictionary<string, string>
        {
            {"Type", ex.GetType().AssemblyQualifiedName! },
            {"Message", ex.Message },
            {"StackTrace", ex.StackTrace! }
        };

        ConsumedFaultMessage faultMessage = new()
        {
            Id = message.Id,
            Payload = message.Payload,
            PrioritizerKey = message.PrioritizerKey,
            Type = message.Type,
            UtcCreationDate = DateTime.UtcNow,
            Exception = JsonSerializer.Serialize(error, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            }),
        };

        await faultService.Save(faultMessage, ct);
        await unitOfWork.SaveChanges(ct);
    }
}

internal sealed class ConsumeResult
{
    public bool ShouldBreakContinuation { get; set; }

    public static ConsumeResult Break => new()
    {
        ShouldBreakContinuation = true,
    };

    public static ConsumeResult NotBreak => new()
    {
        ShouldBreakContinuation = false,
    };
}
