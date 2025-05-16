namespace TaskTracker.Application.DTOs.CheckListItem;

public class CheckListItemDto
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
    public bool? IsCompleted { get; set; }
    public int? Position { get; set; }
    public Guid? CheckListId { get; set; }
}