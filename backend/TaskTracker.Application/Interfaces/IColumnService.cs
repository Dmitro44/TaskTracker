using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Interfaces;

public interface IColumnService
{
    Task Create(ColumnDto dto, CancellationToken ct);
    Task<IEnumerable<ColumnDto>> GetColumns(Guid boardId, CancellationToken ct);
}