using TaskTracker.Application.DTOs.Card;

namespace TaskTracker.Application.DTOs.Column;

public class ColumnFullDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Position { get; set; }
    public IEnumerable<CardDto> Cards { get; set; }
}