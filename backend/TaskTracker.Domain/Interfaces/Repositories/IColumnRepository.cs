using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Interfaces.Repositories;

public interface IColumnRepository
{
    Task<Column?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Column column, CancellationToken ct);
    Task<IEnumerable<Column>> GetAllByBoardAsync(Guid boardId, CancellationToken ct);
}