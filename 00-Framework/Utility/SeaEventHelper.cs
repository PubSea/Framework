using PubSea.Framework.Events;
using PubSea.Framework.Services.Abstractions;

namespace PubSea.Framework.Utility;

public static class SeaEventHelper
{
    private static ISnowflakeService? _snowflakeService;
    private static IDateTimeService? _dateTimeService;

    public static void Init(ISnowflakeService snowflakeService, IDateTimeService dateTimeService)
    {
        _snowflakeService ??= snowflakeService;
        _dateTimeService ??= dateTimeService;
    }

    public static void InitializeEvents(params IEnrichedIntegrationEvent[] events)
    {
        var eventId = _snowflakeService!.CreateId();
        var publishedUtcDateTime = _dateTimeService!.UtcNow.DateTime;

        foreach (var @event in events)
        {
            @event.EventId = eventId++;
            @event.PublishedUtcDateTime = publishedUtcDateTime;
        }
    }

    public static void InitializeEvents(params IEnrichedDomainEvent[] events)
    {
        var eventId = _snowflakeService!.CreateId();
        var publishedUtcDateTime = _dateTimeService!.UtcNow.DateTime;

        foreach (var @event in events)
        {
            @event.EventId = eventId++;
            @event.PublishedUtcDateTime = publishedUtcDateTime;
        }
    }
}