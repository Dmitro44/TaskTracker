namespace TaskTracker.Domain.Entities;

public class CardAssignee
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public Card Card { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime AssignedAt { get; set; }
}