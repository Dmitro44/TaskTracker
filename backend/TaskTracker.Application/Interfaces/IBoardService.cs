using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Interfaces;

public interface IBoardService
{
    Task Create(BoardDto dto, CancellationToken ct);
    Task<IEnumerable<BoardDto>> GetBoards(Guid userId, CancellationToken ct);
}