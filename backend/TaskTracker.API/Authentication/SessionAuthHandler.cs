using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Serialization.Json;
using TaskTracker.Application.DTOs;

namespace TaskTracker.API.Authentication;

public class SessionAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IDistributedCache _distributedCache;
    private const string SessionIdCookieName = "session-id";
    private const string AuthenticationScheme = "RedisSession";

    public SessionAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IDistributedCache distributedCache)
        : base(options, logger, encoder)
    {
        _distributedCache = distributedCache;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Cookies.TryGetValue(SessionIdCookieName, out var sessionId))
        {
            return AuthenticateResult.Fail("Session cookie not found");
        }

        var sessionJsonData = await _distributedCache.GetStringAsync($"session:{sessionId}");

        if (string.IsNullOrEmpty(sessionJsonData))
        {
            return AuthenticateResult.Fail("Session expired or invalid");
        }

        try
        {
            var serializer = new JsonSerializer();
            var sessionData = serializer.Deserialize<UserDto>(sessionJsonData);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, sessionData.Id.ToString()),
                new Claim(ClaimTypes.Name, sessionData.Username),
                new Claim(ClaimTypes.Email, sessionData.Email)
            };

            var identity = new ClaimsIdentity(claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

            await _distributedCache.RefreshAsync($"session:{sessionId}");

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail($"Session authentication failed: {ex.Message}");
        }
    }
}