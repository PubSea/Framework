namespace RestApi.Services;

public sealed class TestService : ITestService
{
    public async Task<string> GetName()
    {
        return await Task.FromResult("Sea");
    }
}