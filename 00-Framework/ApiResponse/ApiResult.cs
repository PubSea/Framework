namespace PubSea.Framework.ApiResponse;

/// <summary>
/// A wrapper on final api response to unify all responses.
/// </summary>
public sealed class ApiResult
{
    private ApiResult()
    { }

    public object Result { get; private set; } = new();

    public static ApiResult From(object obj)
    {
        return new ApiResult
        {
            Result = obj
        };
    }
}

/// <summary>
/// A generic wrapper on final api response to unify all responses.
/// </summary>
public sealed class ApiResult<T> where T : new()
{
    public T Result { get; set; } = new();
}
