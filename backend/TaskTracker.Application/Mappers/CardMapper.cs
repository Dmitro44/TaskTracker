using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;
using Card = TaskTracker.Domain.Entities.Card;


namespace TaskTracker.Application.Mappers;

public class CardMapper : IGenericMapper<CardDto, Card>
{
    public Card ToEntity(CardDto source)
    {
        return new Card
        {
            Id = source.Id,
            Title = source.Title ?? "",
            Position = source.Position!.Value,
            ColumnId = source.ColumnId!.Value
        };
    }

    public CardDto ToDto(Card source)
    {
        return new CardDto
        {
            Id = source.Id,
            Title = source.Title,
            Position = source.Position,
            ColumnId = source.ColumnId
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