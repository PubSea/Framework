using HttpClientToCurl.Extensions;

namespace PubSea.Framework.Http;

public static class CurlRequestGenerator
{
    private static readonly HttpClient _httpClient = new();

    public static string Generate(HttpRequestMessage rq)
    {
        return _httpClient.GenerateCurlInString(rq);
    }
}
