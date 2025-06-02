using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IUserService
{
    Task Register(UserDto dto, string password, CancellationToken ct);
    Task<UserDto> ValidateCredentials(string email, string password, CancellationToken ct);
    Task<UserDto> GetById(Guid userId, CancellationToken ct);
}