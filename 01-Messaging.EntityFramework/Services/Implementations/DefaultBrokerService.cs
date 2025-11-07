using Microsoft.Extensions.Logging;
using PubSea.Messaging.EntityFramework.Models;
using PubSea.Messaging.EntityFramework.Services.Abstractions;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PubSea.Messaging.EntityFramework.Services.Implementations;

internal class DefaultBrokerService : IBrokerService
{
    private readonly ILogger<DefaultBrokerService> _logger;

    public DefaultBrokerService(ILogger<DefaultBrokerService> logger)
    {
        _logger = logger;
    }

    Task IBrokerService.TransportMessages(ICollection<OutboxMessage> messages, CancellationToken ct)
    {
        if (messages.Count == 0)
        {
            return Task.CompletedTask;
        }

        var json = JsonSerializer.Serialize(messages, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        });

        _logger.LogInformation("Events published to broker. {Events}", json);

        return Task.CompletedTask;
    }
}