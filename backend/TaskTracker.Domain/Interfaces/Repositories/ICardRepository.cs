using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Interfaces.Repositories;

public interface ICardRepository
{
    Task<Card?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Card card, CancellationToken ct);
    Task UpdateAsync(Card card, CancellationToken ct);
    Task<IEnumerable<Card>> GetAllByColumnAsync(Guid columnId, CancellationToken ct);
    Task<IEnumerable<Card>> GetAllAsync(CancellationToken ct);
    Task<IEnumerable<Card>> GetCardsByColumns(IEnumerable<Guid> columnIds, CancellationToken ct);
}