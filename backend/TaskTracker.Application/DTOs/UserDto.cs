namespace TaskTracker.Application.DTOs;

public class UserDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
}