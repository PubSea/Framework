namespace PubSea.Framework.Utility;

public static class ExceptionHelper
{
    /// <summary>
    /// It could be useful to ignore some error codes and do not show the real reason of exception to end user
    /// </summary>
    public static List<int> ExcludedExCodes { get; set; } = [];
}
