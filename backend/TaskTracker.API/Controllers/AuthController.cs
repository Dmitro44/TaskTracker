using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Serialization.Json;
using TaskTracker.Application.Interfaces;
using TaskTracker.API.Contracts.Users.Requests;
using TaskTracker.Application.DTOs;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IDistributedCache _distributedCache;

    public AuthController(IUserService userService, IDistributedCache distributedCache)
    {
        _userService = userService;
        _distributedCache = distributedCache;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
    {
        var userDto = new UserDto
        {
            Username = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };
        
        await _userService.Register(userDto, request.Password, ct);

        return Ok();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken ct)
    {
        try
        {
            var userDto = await _userService.ValidateCredentials(request.Email, request.Password, ct);

            string sessionId = Guid.NewGuid().ToString();

            var serializer = new JsonSerializer();

            await _distributedCache.SetStringAsync(
                $"session:{sessionId}",
                serializer.Serialize(userDto),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                },
                ct);

            HttpContext.Response.Cookies.Append("session-id", sessionId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromHours(24)
            });

            return Ok();
        }
        catch (Exception e)
        {
            return Unauthorized(e);
        }
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout(CancellationToken ct)
    {
        if (Request.Cookies.TryGetValue("session-id", out var sessionId))
        {
            await _distributedCache.RemoveAsync($"session:{sessionId}", ct);
            
            Response.Cookies.Delete("session-id");
        }
        
        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok();
    }
}