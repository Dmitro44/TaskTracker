using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;
using TaskTracker.Application.DTOs.Column;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappers;

public static class UpdateDomainFromDtoMapper
{
    public static Card UpdateFromDto(this Card card, CardDto updateDto)
    {
        card.Title = updateDto.Title ?? card.Title;
        card.Position = updateDto.Position ?? card.Position;
        card.ColumnId = updateDto.ColumnId ?? card.ColumnId;
        return card;
    }

    public static CheckListItem UpdateFromDto(this CheckListItem checkListItem, CheckListItemDto updateDto)
    {
        checkListItem.Text = updateDto.Text ?? checkListItem.Text;
        checkListItem.Position = updateDto.Position ?? checkListItem.Position;
        checkListItem.CheckListId = updateDto.CheckListId ?? checkListItem.CheckListId;
        checkListItem.IsCompleted = updateDto.IsCompleted ?? checkListItem.IsCompleted;
        return checkListItem;
    }

    public static CheckList UpdateFromDto(this CheckList checkList, CheckListDto updateDto)
    {
        checkList.Title = updateDto.Title ?? checkList.Title;
        checkList.CardId = updateDto.CardId ?? checkList.CardId;
        return checkList;
    }

    public static Column UpdateFromDto(this Column column, ColumnShortDto updateDto)
    {
        column.Title = updateDto.Title ?? column.Title;
        column.Position = updateDto.Position ?? column.Position;
        column.BoardId = updateDto.BoardId ?? column.BoardId;
        return column;
    }
}