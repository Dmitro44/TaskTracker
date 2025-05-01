using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class Activity
{
    public Guid Id { get; set; }
    public ActivityType Type { get; set; }
    public string Description { get; set; }
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public Guid? CardId { get; set; }
    public Guid? ColumnId { get; set; }
    public DateTime CreatedAt { get; set; }
}