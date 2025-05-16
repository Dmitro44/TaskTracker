namespace TaskTracker.Application.DTOs.Card;

public class CardDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public int? Position { get; set; }
    public Guid? ColumnId { get; set; }
}