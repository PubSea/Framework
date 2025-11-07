using PubSea.Framework.Events;
using RestApi.Services;

namespace RestApi.Events;

public sealed class UserCreationHandler : IEventHandler<UserCreated>
{
    private readonly ITestService _testService;
    private readonly ILogger<UserCreationHandler> _logger;

    public UserCreationHandler(ITestService testService, ILogger<UserCreationHandler> logger)
    {
        _testService = testService;
        _logger = logger;
    }

    async Task IEventHandler<UserCreated>.Handle(UserCreated @event, CancellationToken ct)
    {
        var name = await _testService.GetName();
        _logger.LogInformation($"UserCreationHandler called - Name: {name}");

        await Task.Delay(500, ct);
    }
}
