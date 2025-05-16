using TaskTracker.Application.DTOs.Column;

namespace TaskTracker.Application.DTOs.Board;

public class BoardFullDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public bool IsPublic { get; set; }
    public List<ColumnFullDto> Columns { get; set; }
}
