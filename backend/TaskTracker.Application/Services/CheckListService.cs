using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Application.Services;

public class CheckListService : ICheckListService
{
    private readonly ICheckListRepository _checkListRepository;
    private readonly IGenericMapper<CheckListDto, CheckList> _checkListMapper;
    private readonly IGenericMapper<CheckListItemDto, CheckListItem> _checkListItemMapper;

    public CheckListService(ICheckListRepository checkListRepository, IGenericMapper<CheckListDto, CheckList> checkListMapper, IGenericMapper<CheckListItemDto, CheckListItem> checkListItemMapper)
    {
        _checkListRepository = checkListRepository;
        _checkListMapper = checkListMapper;
        _checkListItemMapper = checkListItemMapper;
    }
    
    public async Task CreateCheckList(CheckListDto dto, CancellationToken ct)
    {
        var checkList = _checkListMapper.ToEntity(dto);

        await _checkListRepository.AddAsync(checkList, ct);
    }

    public async Task CreateCheckListItem(CheckListItemDto checkListItemDto, CancellationToken ct)
    {
        var checkListItem = _checkListItemMapper.ToEntity(checkListItemDto);
        
        await _checkListRepository.AddItemAsync(checkListItem, ct); 
    }

    public async Task<CheckListItemDto> UpdateCheckListItem(Guid clItemId, CheckListItemDto checkListItemDto, CancellationToken ct)
    {
        var cliToUpdate = await _checkListRepository.GetItemByIdAsync(clItemId, ct);
        if (cliToUpdate is null) throw new InvalidOperationException($"CheckListItem with ID {clItemId} not found");
        
        _checkListItemMapper.MapPartial(checkListItemDto, cliToUpdate);

        await _checkListRepository.UpdateCheckListItemAsync(cliToUpdate, ct);
        
        return _checkListItemMapper.ToDto(cliToUpdate);   
    }
}