using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

namespace PubSea.Framework.Http.HealthCheck;

internal class CurlHealthCheck : IHealthCheck
{
    public const string HealthCheck = "curl_health_check";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _host;

    public CurlHealthCheck(IHttpClientFactory httpClientFactory, string host)
    {
        _httpClientFactory = httpClientFactory;
        _host = host;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_host);

            var result = await client.GetAsync(string.Empty, ct);

            if (result.StatusCode < HttpStatusCode.InternalServerError)
            {
                return HealthCheckResult.Healthy("Ready");
            }

            return HealthCheckResult.Unhealthy(result.StatusCode.ToString());

        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
    }
}