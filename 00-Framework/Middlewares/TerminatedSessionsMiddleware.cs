using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PubSea.Framework.Extensions;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;

namespace PubSea.Framework.Middlewares;

internal sealed class TerminatedSessionsMiddleware
{
    private readonly RequestDelegate _next;

    public TerminatedSessionsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context,
        [FromKeyedServices(SeaSloExtensions.LOGOUT_CONNECTION_MULTIPLEXER)] IConnectionMultiplexer cm,
        ILogger<TerminatedSessionsMiddleware> logger)
    {
        var idToken = await context.GetTokenAsync("id_token");

        if (string.IsNullOrWhiteSpace(idToken))
        {
            await _next(context);
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(idToken);
        var sessionId = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            await _next(context);
            return;
        }

        var db = cm.GetDatabase(SeaSloExtensions.LOGOUT_CONNECTION_MULTIPLEXER_DB);
        var key = $"{SeaSloExtensions.LOGOUT_CONNECTION_MULTIPLEXER_PREFIX}{sessionId}";
        var exists = await db.KeyExistsAsync(key);

        if (!exists)
        {
            await _next(context);
            return;
        }

        await Signout(context);
    }

    private static async Task Signout(HttpContext context)
    {
        await context.SignOutAsync("Cookies");
        await context.SignOutAsync("oidc");
    }
}
