using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Application.Services;

public class ColumnService : IColumnService
{
    private readonly IGenericMapper<ColumnDto, Column> _columnMapper;
    private readonly IColumnRepository _columnRepository;

    public ColumnService(IGenericMapper<ColumnDto, Column> columnMapper, IColumnRepository columnRepository)
    {
        _columnMapper = columnMapper;
        _columnRepository = columnRepository;
    }
    
    public async Task Create(ColumnDto dto, CancellationToken ct)
    {
        var column = _columnMapper.ToEntity(dto);
        
        await _columnRepository.AddAsync(column, ct);
    }

    public async Task<IEnumerable<ColumnDto>> GetColumns(Guid boardId, CancellationToken ct)
    {
        var columns = await _columnRepository.GetAllByBoardAsync(boardId, ct);
        
        return columns.Select(c => _columnMapper.ToDto(c));
    }
}