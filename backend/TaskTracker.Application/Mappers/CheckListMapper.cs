using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappers;

public class CheckListMapper : IGenericMapper<CheckListDto, CheckList>
{
    public CheckList ToEntity(CheckListDto source)
    {
        return new CheckList
        {
            Id = source.Id,
            Title = source.Title!,
            CardId = source.CardId!.Value
        };
    }

    public CheckListDto ToDto(CheckList source)
    {
        return new CheckListDto
        {
            Id = source.Id,
            Title = source.Title,
            CardId = source.CardId
        };
    }

    public void MapPartial(CheckListDto source, CheckList destination)
    {
        if (source.Title != null)
            destination.Title = source.Title!;
        if (source.CardId.HasValue)
            destination.CardId = source.CardId.Value;
    }
}