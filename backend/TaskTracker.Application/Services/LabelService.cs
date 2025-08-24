using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mappers;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Application.Services;

public class LabelService : ILabelService
{
    private readonly ILabelRepository _labelRepository;

    public LabelService(ILabelRepository labelRepository)
    {
        _labelRepository = labelRepository;
    }
    
    public async Task Create(LabelDto dto, CancellationToken ct)
    {
        var label = dto.ToEntity();

        await _labelRepository.AddAsync(label, ct);
    }

    public async Task<IEnumerable<LabelDto>> GetLabels(Guid boardId, CancellationToken ct)
    {
        var labels = await _labelRepository.GetAllByBoardAsync(boardId, ct);
        
        return labels.Select(l => l.ToDto());
    }

    public async Task AttachLabelToCard(Guid cardId, Guid labelId, CancellationToken ct)
    {
        await _labelRepository.AttachLabelToCardAsync(cardId, labelId, ct);
    }

    public async Task RemoveLabelFromCard(Guid cardId, Guid labelId, CancellationToken ct)
    {
        await _labelRepository.RemoveLabelFromCardAsync(cardId, labelId, ct);
    }

    public async Task<IEnumerable<LabelDto>> GetLabelsForCard(Guid cardId, CancellationToken ct)
    {
        var labels = await _labelRepository.GetAllByCard(cardId, ct);
        
        return labels.Select(l => l.ToDto());
    }
}