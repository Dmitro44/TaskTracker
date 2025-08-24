using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Board;
using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;
using TaskTracker.Application.DTOs.Column;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappers;

public static class DomainToDtoMapper
{
    public static BoardShortDto ToDto(this Board source)
    {
        return new BoardShortDto
        {
            Id = source.Id,
            Title = source.Title,
            IsPublic = source.IsPublic,
            OwnerId = source.OwnerId
        };
    }

    public static CardDto ToDto(this Card source)
    {
        return new CardDto
        {
            Id = source.Id,
            Title = source.Title,
            Position = source.Position,
            ColumnId = source.ColumnId,
            Labels = source.CardLabels
                .Select(cl => cl.Label.ToDto())
                .ToList(),
        };
    }

    public static LabelDto ToDto(this Label source)
    {
        return new LabelDto
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            BoardId = source.BoardId,
        };
    }

    public static CheckListDto ToDto(this CheckList source)
    {
        return new CheckListDto
        {
            Id = source.Id,
            Title = source.Title,
            CardId = source.CardId,
            Items = source.CheckListItems
                .Select(cli => cli.ToDto())
                .ToList()
        };
    }

    public static CheckListItemDto ToDto(this CheckListItem source)
    {
        return new CheckListItemDto
        {
            Id = source.Id,
            Text = source.Text,
            Position = source.Position,
            CheckListId = source.CheckListId
        };
    }

    public static ColumnShortDto ToDto(this Column source)
    {
        return new ColumnShortDto
        {
            Id = source.Id,
            Title = source.Title,
            Position = source.Position,
            BoardId = source.BoardId
        };
    }

    public static UserDto ToDto(this User source)
    {
        return new UserDto
        {
            Id = source.Id,
            Username = source.Username,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Email = source.Email
        };
    }
}