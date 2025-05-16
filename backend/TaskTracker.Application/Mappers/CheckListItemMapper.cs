using TaskTracker.Application.DTOs.CheckListItem;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappers;

public class CheckListItemMapper : IGenericMapper<CheckListItemDto, CheckListItem>
{
    public CheckListItem ToEntity(CheckListItemDto source)
    {
        return new CheckListItem
        {
            Id = source.Id,
            Text = source.Text!,
            Position = source.Position!.Value,
            CheckListId = source.CheckListId!.Value
        };
    }

    public CheckListItemDto ToDto(CheckListItem source)
    {
        return new CheckListItemDto
        {
            Id = source.Id,
            Text = source.Text,
            Position = source.Position,
            CheckListId = source.CheckListId
        };
    }

    public void MapPartial(CheckListItemDto source, CheckListItem destination)
    {
        if (source.Text != null)
            destination.Text = source.Text;
        if (source.Position.HasValue)
            destination.Position = source.Position.Value;
        if (source.CheckListId.HasValue)
            destination.CheckListId = source.CheckListId.Value;
        if (source.IsCompleted.HasValue)
            destination.IsCompleted = source.IsCompleted.Value;
    }
}