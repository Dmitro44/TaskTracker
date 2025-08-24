using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mappers;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Application.Services;

public class CheckListService : ICheckListService
{
    private readonly ICheckListRepository _checkListRepository;

    public CheckListService(ICheckListRepository checkListRepository)
    {
        _checkListRepository = checkListRepository;
    }
    
    public async Task CreateCheckList(CheckListDto dto, CancellationToken ct)
    {
        var checkList = dto.ToEntity();

        await _checkListRepository.AddAsync(checkList, ct);
    }

    public async Task CreateCheckListItem(CheckListItemDto checkListItemDto, CancellationToken ct)
    {
        var checkListItem = checkListItemDto.ToEntity();
        
        await _checkListRepository.AddItemAsync(checkListItem, ct); 
    }

    public async Task<CheckListItemDto> UpdateCheckListItem(Guid clItemId, CheckListItemDto checkListItemDto, CancellationToken ct)
    {
        var cliToUpdate = await _checkListRepository.GetItemByIdAsync(clItemId, ct);
        if (cliToUpdate is null) throw new InvalidOperationException($"CheckListItem with ID {clItemId} not found");

        cliToUpdate.UpdateFromDto(checkListItemDto);

        await _checkListRepository.UpdateCheckListItemAsync(cliToUpdate, ct);
        
        return cliToUpdate.ToDto();   
    }
}