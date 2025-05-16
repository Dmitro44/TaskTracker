using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;

namespace TaskTracker.Application.Interfaces;

public interface ICheckListService
{
    Task CreateCheckList(CheckListDto dto, CancellationToken ct);

    Task CreateCheckListItem(CheckListItemDto checkListItemDto, CancellationToken ct);
    Task<CheckListItemDto> UpdateCheckListItem(Guid clItemId, CheckListItemDto checkListItemDto, CancellationToken ct);
}