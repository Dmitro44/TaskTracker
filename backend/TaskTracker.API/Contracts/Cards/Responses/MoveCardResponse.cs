namespace TaskTracker.API.Contracts.Cards.Responses;

public record MoveCardResponse(
    int Position,
    Guid ColumnId);