using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Column;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappers;

public class ColumnMapper : IGenericMapper<ColumnShortDto, Column>
{
    public Column ToEntity(ColumnShortDto source)
    {
        return new Column
        {
            Id = source.Id,
            Title = source.Title ?? "",
            Position = source.Position!.Value,
            BoardId = source.BoardId!.Value
        };
    }

    public ColumnShortDto ToDto(Column source)
    {
        return new ColumnShortDto
        {
            Id = source.Id,
            Title = source.Title,
            Position = source.Position,
            BoardId = source.BoardId
        };
    }

    public void MapPartial(ColumnShortDto source, Column destination)
    {
        if (source.Title != null)
            destination.Title = source.Title;
        if (source.Position.HasValue)
            destination.Position = source.Position.Value;
        if (source.BoardId.HasValue)
            destination.BoardId = source.BoardId.Value;
    }
}