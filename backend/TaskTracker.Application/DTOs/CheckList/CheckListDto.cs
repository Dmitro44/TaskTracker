namespace TaskTracker.Application.DTOs.CheckList;

public class CheckListDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Guid? CardId { get; set; }
}