namespace TaskTracker.Application.DTOs;

public class BoardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public bool IsPublic { get; set; }
    public Guid OwnerId { get; set; }
}