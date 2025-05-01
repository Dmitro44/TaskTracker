using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Interfaces;

public interface ILabelService
{
    Task Create(LabelDto dto, CancellationToken ct);
    Task<IEnumerable<LabelDto>> GetLabels(Guid boardId, CancellationToken ct);
}