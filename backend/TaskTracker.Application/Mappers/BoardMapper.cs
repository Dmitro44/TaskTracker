using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappers;

public class BoardMapper : IGenericMapper<BoardDto, Board>
{
    public Board ToEntity(BoardDto source)
    {
        return new Board
        {
            Id = source.Id,
            Title = source.Title,
            IsPublic = source.IsPublic,
            OwnerId = source.OwnerId
        };
    }

    public BoardDto ToDto(Board source)
    {
        return new BoardDto
        {
            Id = source.Id,
            Title = source.Title,
            IsPublic = source.IsPublic,
            OwnerId = source.OwnerId
        };
    }

    public void MapPartial(BoardDto source, Board destination)
    {
        throw new NotImplementedException();
    }
}