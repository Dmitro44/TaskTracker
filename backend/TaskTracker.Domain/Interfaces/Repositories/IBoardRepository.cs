using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Interfaces.Repositories;

public interface IBoardRepository
{
    Task<Board?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Board board, CancellationToken ct);
    Task<IEnumerable<Board>> GetAllByUserAsync(Guid userId, CancellationToken ct);
}