using TaskTracker.Domain.Interfaces;

namespace TaskTracker.Domain.Entities;

public class Card : IAuditableEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Position { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid ListId { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }

    public List<CheckList> CheckLists { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<Attachment> Attachments { get; set; } = new();
}