using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.API.Contracts.Users.Requests;
using TaskTracker.Application.DTOs;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
    {
        var userDto = new UserDto
        {
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };
        
        await _userService.Register(userDto, request.Password, ct);

        return Ok();
    }
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken ct)
    {
        var token = await _userService.Login(request.Email, request.Password, ct);

        var context = HttpContext;
        
        context.Response.Cookies.Append("jwt-cookie", token);
        
        return Ok();
    }
}