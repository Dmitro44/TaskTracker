namespace TaskTracker.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Guid CardId { get; set; }
    public Card Card { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }
}