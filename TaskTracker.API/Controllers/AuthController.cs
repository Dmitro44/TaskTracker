using Microsoft.AspNetCore.Mvc;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    public Task<IActionResult> Login()
    {
        
    }
}