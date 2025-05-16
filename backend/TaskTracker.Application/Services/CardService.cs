using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Interfaces.Repositories;
using Card = TaskTracker.Domain.Entities.Card;

namespace TaskTracker.Application.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IGenericMapper<CardDto, Card> _cardMapper;
    private readonly ILabelService _labelService;
    private readonly ICheckListService _checkListService;

    public CardService(ICardRepository cardRepository, IGenericMapper<CardDto, Card> cardMapper, ILabelService labelService, ICheckListService checkListService)
    {
        _cardRepository = cardRepository;
        _cardMapper = cardMapper;
        _labelService = labelService;
        _checkListService = checkListService;
    }
    
    public async Task<Card> CreateCard(CardDto dto, CancellationToken ct)
    {
        var card = _cardMapper.ToEntity(dto);
        
        await _cardRepository.AddAsync(card, ct);

        return card;
    }

    public async Task<IEnumerable<CardDto>> GetAllCards(Guid boardId, CancellationToken ct)
    {
        var cards = await _cardRepository.GetAllAsync(ct);
        
        return cards.Select(c => _cardMapper.ToDto(c));
    }

    public async Task<CardDto> UpdateCard(CardDto dto, CancellationToken ct)
    {
        var card = await _cardRepository.GetByIdAsync(dto.Id, ct);
        if (card is null) throw new InvalidOperationException($"Card with ID {dto.Id} not found");

        _cardMapper.MapPartial(dto, card);
        
        await _cardRepository.UpdateAsync(card, ct);
        
        return _cardMapper.ToDto(card);
    }

    public async Task AddLabelToCard(Guid cardId, Guid labelId, CancellationToken ct)
    {
        await _labelService.AttachLabelToCard(cardId, labelId, ct);
    }

    public async Task RemoveLabelFromCard(Guid cardId, Guid labelId, CancellationToken ct)
    {
        await _labelService.RemoveLabelFromCard(cardId, labelId, ct);    
    }

    public async Task AddCheckList(CheckListDto checkListDto, CancellationToken ct)
    {
        await _checkListService.CreateCheckList(checkListDto, ct);
    }

    public async Task AddCheckListItem(CheckListItemDto checkListItemDto, CancellationToken ct)
    {
        await _checkListService.CreateCheckListItem(checkListItemDto, ct);
    }

    public async Task<CheckListItemDto> UpdateCheckListItem(Guid clItemId, CheckListItemDto checkListItemDto, CancellationToken ct)
    {
        return await _checkListService.UpdateCheckListItem(clItemId, checkListItemDto, ct);
    }

    public async Task<IDictionary<Guid, IEnumerable<CardDto>>> GetCardsByColumns(IEnumerable<Guid> columnIds,
        CancellationToken ct)
    {
        var cards = await _cardRepository.GetCardsByColumns(columnIds, ct);
        
        return cards
            .GroupBy(c => c.ColumnId)
            .ToDictionary(g => g.Key, g => g.Select(card => _cardMapper.ToDto(card)));
    }
}