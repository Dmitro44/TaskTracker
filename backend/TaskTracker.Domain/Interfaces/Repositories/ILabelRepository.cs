using System.Reflection.Emit;
using Label = TaskTracker.Domain.Entities.Label;

namespace TaskTracker.Domain.Interfaces.Repositories;

public interface ILabelRepository
{
    Task<Label?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Label label, CancellationToken ct);
    Task<IEnumerable<Label>> GetAllByBoardAsync(Guid boardId, CancellationToken ct);
}