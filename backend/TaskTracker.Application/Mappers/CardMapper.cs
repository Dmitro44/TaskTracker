using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;
using Card = TaskTracker.Domain.Entities.Card;


namespace TaskTracker.Application.Mappers;

public class CardMapper : IGenericMapper<CardDto, Card>
{
    private readonly IGenericMapper<LabelDto, Label> _labelMapper;

    public CardMapper(IGenericMapper<LabelDto, Label> labelMapper)
    {
        _labelMapper = labelMapper;
    }

    public Card ToEntity(CardDto source)
    {
        return new Card
        {
            Id = source.Id,
            Title = source.Title ?? "",
            Position = source.Position!.Value,
            ColumnId = source.ColumnId!.Value,
        };
    }

    public CardDto ToDto(Card source)
    {
        return new CardDto
        {
            Id = source.Id,
            Title = source.Title,
            Position = source.Position,
            ColumnId = source.ColumnId,
            Labels = source.CardLabels
                .Select(cl => _labelMapper.ToDto(cl.Label))
                .ToList(),
        };
    }

    public void MapPartial(CardDto source, Card destination)
    {
        if (source.Title != null)
            destination.Title = source.Title;
        if (source.Position.HasValue)
            destination.Position = source.Position.Value;
        if (source.ColumnId.HasValue)
            destination.ColumnId = source.ColumnId.Value;
    }
}