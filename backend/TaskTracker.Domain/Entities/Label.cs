namespace TaskTracker.Domain.Entities;

public class Label
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public Guid BoardId { get; set; }
    public Board Board { get; set; }

    public List<CardLabel> CardLabels { get; set; } = new();
}