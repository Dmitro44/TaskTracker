using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Board;
using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;
using TaskTracker.Application.DTOs.Column;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappers;

public static class DtoToDomainMapper
{
    public static Board ToEntity(this BoardShortDto source)
    {
        return new Board
        {
            Id = source.Id,
            Title = source.Title,
            IsPublic = source.IsPublic,
            OwnerId = source.OwnerId
        };
    }

    public static Card ToEntity(this CardDto source)
    {
        return new Card
        {
            Id = source.Id,
            Title = source.Title ?? "",
            Position = source.Position!.Value,
            ColumnId = source.ColumnId!.Value,
        };
    }

    public static Label ToEntity(this LabelDto source)
    {
        return new Label
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            BoardId = source.BoardId,
        };
    }

    public static User ToEntity(this UserDto source)
    {
        return new User
        {
            Id = source.Id,
            Username = source.Username,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Email = source.Email
        };
    }

    public static Column ToEntity(this ColumnShortDto source)
    {
        return new Column
        {
            Id = source.Id,
            Title = source.Title ?? "",
            Position = source.Position!.Value,
            BoardId = source.BoardId!.Value
        };
    }

    public static CheckList ToEntity(this CheckListDto source)
    {
        return new CheckList
        {
            Id = source.Id,
            Title = source.Title!,
            CardId = source.CardId!.Value
        };
    }

    public static CheckListItem ToEntity(this CheckListItemDto source)
    {
        return new CheckListItem
        {
            Id = source.Id,
            Text = source.Text!,
            Position = source.Position!.Value,
            CheckListId = source.CheckListId!.Value
        };
    }
}