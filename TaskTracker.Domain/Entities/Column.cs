using TaskTracker.Domain.Interfaces;

namespace TaskTracker.Domain.Entities;

public class Column : IAuditableEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Position { get; set; }
    public Guid BoardId { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }

    public List<Card> Cards { get; set; } = new();
}