using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Interfaces.Auth;

public interface IJwtProvider
{
    string GenerateToken(User user);
}