using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Column;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IColumnService
{
    Task<Column> Create(ColumnShortDto shortDto, CancellationToken ct);
    Task<IEnumerable<ColumnShortDto>> GetColumns(Guid boardId, CancellationToken ct);
    Task<ColumnShortDto> MoveColumns(ColumnShortDto columnDto, CancellationToken ct);
    Task UpdateColumn(ColumnShortDto updateColumnDto, CancellationToken ct);
}