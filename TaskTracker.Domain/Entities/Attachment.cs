namespace TaskTracker.Domain.Entities;

public class Attachment
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; }
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
}