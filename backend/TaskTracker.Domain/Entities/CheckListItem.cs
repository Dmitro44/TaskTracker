namespace TaskTracker.Domain.Entities;

public class CheckListItem
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool IsCompleted { get; set; }
    public int Position { get; set; }
    public Guid CheckListId { get; set; }
    public CheckList CheckList { get; set; }
}