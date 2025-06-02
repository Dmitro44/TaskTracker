namespace TaskTracker.Application.DTOs;

public class LabelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public Guid BoardId { get; set; }
    public Guid? CardId { get; set; }
}