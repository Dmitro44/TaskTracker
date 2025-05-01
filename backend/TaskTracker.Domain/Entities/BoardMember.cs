using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class BoardMember
{
    public Guid Id { get; set; }
    public MemberRole MemberRole { get; set; }
    public Guid BoardId { get; set; }
    public Board Board { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime JoinedAt { get; set; }
}