using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
}