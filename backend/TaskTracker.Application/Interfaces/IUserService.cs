using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IUserService
{
    Task Register(UserDto dto, string password, CancellationToken ct);
    Task<string> Login(string email, string password, CancellationToken ct);
    Task<User> GetById(Guid userId, CancellationToken ct);
}