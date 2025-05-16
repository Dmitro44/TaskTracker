using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Interfaces;

public interface ILabelService
{
    Task Create(LabelDto dto, CancellationToken ct);
    Task<IEnumerable<LabelDto>> GetLabels(Guid boardId, CancellationToken ct);
    Task AttachLabelToCard(Guid cardId, Guid labelId, CancellationToken ct);
    Task RemoveLabelFromCard(Guid cardId, Guid labelId, CancellationToken ct);
}