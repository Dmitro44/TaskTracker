using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces.Mapping;
using Label = TaskTracker.Domain.Entities.Label;

namespace TaskTracker.Application.Mappers;

public class LabelMapper : IGenericMapper<LabelDto, Label>
{
    public Label ToEntity(LabelDto source)
    {
        return new Label
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            BoardId = source.BoardId
        };
    }

    public LabelDto ToDto(Label source)
    {
        return new LabelDto
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            BoardId = source.BoardId
        };
    }

    public void MapPartial(LabelDto source, Label destination)
    {
        throw new NotImplementedException();
    }
}