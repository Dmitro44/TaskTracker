using TaskTracker.Domain.Interfaces;

namespace TaskTracker.Domain.Entities;

public class Board : IAuditableEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public bool IsPublic { get; set; }
    public Guid OwnerId { get; set; }
    public User Owner { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    
    public List<Column> Columns { get; set; } = new();
}