namespace TaskTracker.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}