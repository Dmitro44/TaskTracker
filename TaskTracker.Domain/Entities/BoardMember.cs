using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class BoardMember
{
    public MemberRole MemberRole { get; set; }
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; }
}