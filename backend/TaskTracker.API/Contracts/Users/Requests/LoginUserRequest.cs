using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Contracts.Users.Requests;

public record LoginUserRequest(
    [Required] string Email,
    [Required] string Password);