using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TaskTracker.API.Extensions;

public static class ControllerExtension
{
    public static Guid GetCurrentUserId(this ControllerBase controller)
    {
        var userIdClaim = controller.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Guid.Empty;
        }
        
        return userId;
    }   
}