using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Board;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappers;

public class BoardMapper : IGenericMapper<BoardShortDto, Board>
{
    public Board ToEntity(BoardShortDto source)
    {
        return new Board
        {
            Id = source.Id,
            Title = source.Title,
            IsPublic = source.IsPublic,
            OwnerId = source.OwnerId
        };
    }

    public BoardShortDto ToDto(Board source)
    {
        return new BoardShortDto
        {
            Id = source.Id,
            Title = source.Title,
            IsPublic = source.IsPublic,
            OwnerId = source.OwnerId
        };
    }

    public void MapPartial(BoardShortDto source, Board destination)
    {
        throw new NotImplementedException();
    }
}