namespace TaskTracker.Application.DTOs.Column;

public class ColumnShortDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public int? Position { get; set; }
    public Guid? BoardId { get; set; }
}