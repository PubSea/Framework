namespace PubSea.Framework.Utility;

public static class TaskWrapper
{
    public static async Task WrapWhenAll(params Task[] tasks)
    {
        try
        {
            await Task.WhenAll(tasks);
        }
        catch
        { }
    }

    public static async Task WrapWhenAll(IEnumerable<Task> tasks)
    {
        await WrapWhenAll(tasks.ToArray());
    }

    public static void WrapWaitAll(Task[] tasks, int millisecondTimeout, CancellationToken ct)
    {
        try
        {
            Task.WaitAll(tasks, millisecondTimeout, ct);
        }
        catch
        { }
    }
}
