using PubSea.Framework.Exceptions;

namespace PubSea.Framework.Utility;

public static class UrlUtility
{
    public static void ValidateIfUrlIsInternal(string? url)
    {
        if (string.IsNullOrWhiteSpace(url) ||
            ((url.StartsWith('/') || url.StartsWith(@"\")) &&
                !url.StartsWith("//") &&
                !url.StartsWith(@"\\") &&
                !url.StartsWith(@"\/") &&
                !url.StartsWith(@"/\") &&
                !url.Contains(':') && !url.Contains(',') &&
                Uri.TryCreate(url, UriKind.Relative, out _)))
        {
            return;
        }

        throw new SeaException("پارامتر مسیر بازگشتی موجود در url اشتباه است",
            SeaException.INVALID_REDIRECT_URL_CODE, ExceptionStatus.InvalidArgument);
    }
}
