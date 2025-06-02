using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Application.Services;

public class LabelService : ILabelService
{
    private readonly ILabelRepository _labelRepository;
    private readonly IGenericMapper<LabelDto, Label> _labelMapper;

    public LabelService(ILabelRepository labelRepository, IGenericMapper<LabelDto, Label> labelMapper)
    {
        _labelRepository = labelRepository;
        _labelMapper = labelMapper;
    }
    
    public async Task Create(LabelDto dto, CancellationToken ct)
    {
        var label = _labelMapper.ToEntity(dto);

        await _labelRepository.AddAsync(label, ct);
    }

    public async Task<IEnumerable<LabelDto>> GetLabels(Guid boardId, CancellationToken ct)
    {
        var labels = await _labelRepository.GetAllByBoardAsync(boardId, ct);
        
        return labels.Select(l => _labelMapper.ToDto(l));
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
        
        return labels.Select(l => _labelMapper.ToDto(l));
    }
}