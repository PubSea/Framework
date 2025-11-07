using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace PubSea.Framework.Http;

public sealed class HttpErrorLoggerMessageHandler : DelegatingHandler
{
    private readonly ILogger<HttpErrorLoggerMessageHandler> _logger;

    public HttpErrorLoggerMessageHandler(ILogger<HttpErrorLoggerMessageHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage rq, CancellationToken ct)
    {
        var startTime = Stopwatch.GetTimestamp();

        HttpResponseMessage? rs = null;

        try
        {
            rs = await base.SendAsync(rq, ct);
            return rs;
        }
        finally
        {
            if (rs is not null && !rs.IsSuccessStatusCode)
            {
                var curl = CurlRequestGenerator.Generate(rq);
                await rs.Content.LoadIntoBufferAsync(ct);
                var content = await rs.Content.ReadAsStringAsync(ct);

                var stream = await rs.Content.ReadAsStreamAsync(ct);
                stream.Seek(0, SeekOrigin.Begin);

                _logger.LogUnsuccessfullHttpRequest(rs.StatusCode, curl, content);
            }
        }
    }
}
