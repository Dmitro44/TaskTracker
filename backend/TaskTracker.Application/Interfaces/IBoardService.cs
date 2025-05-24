using TaskTracker.Application.DTOs.Board;

namespace TaskTracker.Application.Interfaces;

public interface IBoardService
{
    Task Create(BoardShortDto shortDto, CancellationToken ct);
    Task<IEnumerable<BoardShortDto>> GetBoards(Guid userId, CancellationToken ct);
    Task<BoardFullDto> GetBoardWithColumnsAndCards(Guid boardId, CancellationToken ct);
    Task ArchiveBoard(Guid boardId, Guid userId, CancellationToken ct);
    Task RestoreBoard(Guid boardId, CancellationToken ct);
}