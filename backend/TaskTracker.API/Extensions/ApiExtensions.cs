using Microsoft.AspNetCore.Authentication;
using TaskTracker.API.Authentication;

namespace TaskTracker.API.Extensions;

public static class ApiExtensions
{
    public static void AddRedisSessionAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
        });

        services.AddHttpContextAccessor();

        services.AddAuthentication("RedisSession")
            .AddScheme<AuthenticationSchemeOptions, SessionAuthHandler>("RedisSession", _ => { });
        
        services.AddAuthorization();
    }
}