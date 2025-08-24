using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mappers;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Application.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly ILabelService _labelService;
    private readonly ICheckListService _checkListService;
    private readonly IUserService _userService;

    public CardService(
        ICardRepository cardRepository,
        ILabelService labelService,
        ICheckListService checkListService,
        IUserService userService)
    {
        _cardRepository = cardRepository;
        _labelService = labelService;
        _checkListService = checkListService;
        _userService = userService;
    }
    
    public async Task<Card> CreateCard(CardDto dto, CancellationToken ct)
    {
        var card = dto.ToEntity();
        
        await _cardRepository.AddAsync(card, ct);

        return card;
    }

    public async Task<IEnumerable<CardDto>> GetAllCards(Guid boardId, CancellationToken ct)
    {
        var cards = await _cardRepository.GetAllAsync(ct);
        
        return cards.Select(c => c.ToDto());
    }

    public async Task<CardDto> MoveCards(CardDto dto, CancellationToken ct)
    {
        var card = await _cardRepository.GetByIdAsync(dto.Id, ct);
        if (card is null)
            throw new InvalidOperationException($"Card with ID {dto.Id} not found");

        var columnCards = await _cardRepository.GetAllByColumnAsync(card.ColumnId, ct);

        var oldPosition = card.Position;
        var newPosition = dto.Position;

        foreach (var c in columnCards)
        {
            if (c.Id == card.Id) continue;
            if (oldPosition < newPosition)
            {
                if (c.Position > oldPosition && c.Position <= newPosition)
                    c.Position--;
            }
            else
            {
                if (c.Position >= newPosition && c.Position < oldPosition)
                    c.Position++;
            }
            
            await _cardRepository.UpdateAsync(c, ct);
        }

        card.UpdateFromDto(dto);

        await _cardRepository.UpdateAsync(card, ct);

        return card.ToDto();
    }

    public async Task<CardDto> UpdateCard(CardDto dto, CancellationToken ct)
    {
        var card = await _cardRepository.GetByIdAsync(dto.Id, ct);
        if (card is null) throw new InvalidOperationException($"Card with ID {dto.Id} not found");

        card.UpdateFromDto(dto);
        
        await _cardRepository.UpdateAsync(card, ct);
        
        return card.ToDto();
    }

    public async Task ArchiveCard(Guid cardId, Guid userId, CancellationToken ct)
    {
        var card = await _cardRepository.GetByIdAsync(cardId, ct);
        if (card is null)
            throw new InvalidOperationException($"Card to archive with ID {cardId} not found");
        
        var userDto = await _userService.GetById(userId, ct);
        
        card.IsArchived = true;
        card.ArchivedAt = DateTime.UtcNow;
        card.ArchivedBy = string.Join(" ", userDto.FirstName, userDto.LastName);
        
        await _cardRepository.UpdateAsync(card, ct);
    }

    public async Task RestoreCard(Guid cardId, CancellationToken ct)
    {
        var card = await _cardRepository.GetByIdAsync(cardId, ct);
        if (card is null)
            throw new InvalidOperationException($"Card to restore with ID {cardId} not found");
        
        card.IsArchived = false;
        card.ArchivedAt = null;
        card.ArchivedBy = null;
        
        await _cardRepository.UpdateAsync(card, ct);
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
            .ToDictionary(g => g.Key, g => g.Select(card => card.ToDto()));
    }
}