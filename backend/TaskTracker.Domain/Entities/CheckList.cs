namespace TaskTracker.Domain.Entities;

public class CheckList
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid CardId { get; set; }
    public Card Card { get; set; }

    public List<CheckListItem> CheckListItems { get; set; } = [];
}