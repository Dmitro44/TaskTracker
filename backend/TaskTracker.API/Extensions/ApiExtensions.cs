using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskTracker.API.Authentication;
using TaskTracker.Infrastructure.Auth;

namespace TaskTracker.API.Extensions;

public static class ApiExtensions
{
    public static void AddRedisSessionAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
        });

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        services.AddHttpContextAccessor();

        services.AddAuthentication("RedisSession")
            .AddScheme<AuthenticationSchemeOptions, SessionAuthHandler>("RedisSession", _ => { });
        
        services.AddAuthorization();
    }
}