using PubSea.Framework.Events;
using RestApi.Services;

namespace RestApi.Events;

public sealed class UserModificationHandler : IEventHandler<UserModified>
{
    private readonly ITestService _testService;
    private readonly ILogger<UserCreationHandler> _logger;

    public UserModificationHandler(ITestService testService, ILogger<UserCreationHandler> logger)
    {
        _testService = testService;
        _logger = logger;
    }

    async Task IEventHandler<UserModified>.Handle(UserModified @event, CancellationToken ct)
    {
        var name = await _testService.GetName();
        _logger.LogInformation($"UserModificationHandler called - UserId: {@event.UserId}, Name: {name}");

        await Task.Delay(500, ct);
    }
}