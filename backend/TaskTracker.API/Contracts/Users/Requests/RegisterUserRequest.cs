using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Contracts.Users.Requests;

public record RegisterUserRequest(
    [Required] string UserName,
    [Required] string FirstName,
    [Required] string LastName,
    [Required] string Email,
    [Required] string Password
);