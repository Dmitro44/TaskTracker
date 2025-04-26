namespace TaskTracker.Domain.Entities;

public class CardAssignee
{
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public DateTime AssignedAt { get; set; }
}