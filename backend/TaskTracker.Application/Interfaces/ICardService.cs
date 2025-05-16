using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface ICardService
{
    Task<Card> CreateCard(CardDto dto, CancellationToken ct);
    Task<IEnumerable<CardDto>> GetAllCards(Guid boardId, CancellationToken ct);
    Task<CardDto> UpdateCard(CardDto dto, CancellationToken ct);
    Task AddLabelToCard(Guid cardId, Guid labelId, CancellationToken ct);
    Task<IDictionary<Guid, IEnumerable<CardDto>>> GetCardsByColumns(IEnumerable<Guid> columnIds, CancellationToken ct);
    Task RemoveLabelFromCard(Guid cardId, Guid labelId, CancellationToken ct);
    Task AddCheckList(CheckListDto checkListDto, CancellationToken ct);
    Task AddCheckListItem(CheckListItemDto checkListItemDto, CancellationToken ct);
    Task<CheckListItemDto> UpdateCheckListItem(Guid clItemId, CheckListItemDto checkListItemDto, CancellationToken ct);
}