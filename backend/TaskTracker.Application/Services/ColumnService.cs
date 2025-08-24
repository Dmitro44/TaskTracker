using TaskTracker.Application.DTOs.Column;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mappers;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Application.Services;

public class ColumnService : IColumnService
{
    private readonly IColumnRepository _columnRepository;
    private readonly IUserService _userService;

    public ColumnService(
        IColumnRepository columnRepository,
        IUserService userService)
    {
        _columnRepository = columnRepository;
        _userService = userService;
    }
    
    public async Task<Column> Create(ColumnShortDto shortDto, CancellationToken ct)
    {
        var column = shortDto.ToEntity();
        
        await _columnRepository.AddAsync(column, ct);

        return column;
    }

    public async Task<IEnumerable<ColumnShortDto>> GetColumns(Guid boardId, CancellationToken ct)
    {
        var columns = await _columnRepository.GetAllByBoardAsync(boardId, ct);
        
        return columns.Select(c => c.ToDto());
    }

    public async Task<ColumnShortDto> MoveColumns(ColumnShortDto columnDto, CancellationToken ct)
    {
        var column = await _columnRepository.GetByIdAsync(columnDto.Id, ct);
        if (column is null)
            throw new InvalidOperationException($"Column with ID {columnDto.Id} not found");

        var boardColumns = await _columnRepository.GetAllByBoardAsync(column.BoardId, ct);
        if (boardColumns is null)
            throw new InvalidOperationException($"Columns from board {column.BoardId} not found");
        
        var oldPosition = column.Position;
        var newPosition = columnDto.Position;

        if (oldPosition == newPosition)
            return column.ToDto();

        foreach (var col in boardColumns)
        {
            if (col.Id == columnDto.Id) continue;
            if (oldPosition < newPosition)
            {
                if (col.Position > oldPosition && col.Position <= newPosition)
                    col.Position--;
            }
            else
            {
                if (col.Position >= newPosition && col.Position < oldPosition)
                    col.Position++;
            }
            
            //Better make UpdateManyAsync for all columns
            await _columnRepository.UpdateAsync(col, ct);
        }
        
        column.UpdateFromDto(columnDto);
        
        await _columnRepository.UpdateAsync(column, ct);
        
        return column.ToDto();
    }

    public async Task UpdateColumn(ColumnShortDto updateColumnDto, CancellationToken ct)
    {
        var columnToUpdate = await _columnRepository.GetByIdAsync(updateColumnDto.Id, ct);
        if (columnToUpdate is null)
        {
            throw new InvalidOperationException($"Column with ID {updateColumnDto.Id} not found");
        }
        
        columnToUpdate.UpdateFromDto(updateColumnDto);
        
        await _columnRepository.UpdateAsync(columnToUpdate, ct);
    }

    public async Task ArchiveColumn(Guid columnId, Guid userId, CancellationToken ct)
    {
        var column = await _columnRepository.GetByIdAsync(columnId, ct);
        if (column is null) 
            throw new InvalidOperationException($"Column to archive with ID {columnId} not found");
        
        var userDto = await _userService.GetById(userId, ct);
        
        column.IsArchived = true;
        column.ArchivedAt = DateTime.UtcNow;
        column.ArchivedBy = string.Join(" ", userDto.FirstName, userDto.LastName);
        
        await _columnRepository.UpdateAsync(column, ct);
    }

    public async Task RestoreColumn(Guid columnId, CancellationToken ct)
    {
        var column = await _columnRepository.GetByIdAsync(columnId, ct);
        if (column is null)
            throw new InvalidOperationException($"Column to restore with ID {columnId} not found");
        
        column.IsArchived = false;
        column.ArchivedAt = null;
        column.ArchivedBy = null;
        
        await _columnRepository.UpdateAsync(column, ct);
    }
}