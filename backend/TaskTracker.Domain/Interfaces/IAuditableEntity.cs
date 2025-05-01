namespace TaskTracker.Domain.Interfaces;

public interface IAuditableEntity
{
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}