using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Interfaces.Repositories;

public interface ICardRepository
{
    Task<Card?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Card card, CancellationToken ct);
    Task UpdateAsync(Card card, CancellationToken ct);
    Task<IEnumerable<Card>> GetAllByColumnIdAsync(Guid columnId, CancellationToken ct);
}