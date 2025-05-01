using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappers;

public class ColumnMapper : IGenericMapper<ColumnDto, Column>
{
    public Column ToEntity(ColumnDto source)
    {
        return new Column
        {
            Id = source.Id,
            Title = source.Title,
            Position = source.Position,
            BoardId = source.BoardId
        };
    }

    public ColumnDto ToDto(Column source)
    {
        return new ColumnDto
        {
            Id = source.Id,
            Title = source.Title,
            Position = source.Position,
            BoardId = source.BoardId
        };
    }

    public void MapPartial(ColumnDto source, Column destination)
    {
        throw new NotImplementedException();
    }
}