using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Interfaces;

public interface IUserService
{
    Task Register(UserDto dto, string password, CancellationToken ct);
    Task<string> Login(string email, string password, CancellationToken ct);
}