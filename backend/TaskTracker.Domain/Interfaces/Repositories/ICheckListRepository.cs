using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Interfaces.Repositories;

public interface ICheckListRepository
{
    Task<CheckList?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(CheckList checkList, CancellationToken ct);
    Task AddItemAsync(CheckListItem checkListItem, CancellationToken ct);
    Task<CheckListItem?> GetItemByIdAsync(Guid clItemId, CancellationToken ct);
    Task UpdateCheckListItemAsync(CheckListItem cliToUpdate, CancellationToken ct);
}