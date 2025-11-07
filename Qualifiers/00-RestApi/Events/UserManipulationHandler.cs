using PubSea.Framework.Events;
using RestApi.Services;

namespace RestApi.Events;

public sealed class UserManipulationHandler : IEventHandler<UserCreated>, IEventHandler<UserModified>
{
    private readonly ITestService _testService;
    private readonly ILogger<UserCreationHandler> _logger;

    public UserManipulationHandler(ITestService testService, ILogger<UserCreationHandler> logger)
    {
        _testService = testService;
        _logger = logger;
    }

    async Task IEventHandler<UserCreated>.Handle(UserCreated @event, CancellationToken ct)
    {
        _logger.LogInformation("UserManipulationHandler called for creation");

        await Task.Delay(500, ct);
    }

    async Task IEventHandler<UserModified>.Handle(UserModified @event, CancellationToken ct)
    {
        var name = await _testService.GetName();
        _logger.LogInformation($"UserManipulationHandler called for modification - UserId: {@event.UserId}, Name: {name}");

        await Task.Delay(500, ct);
    }
}
