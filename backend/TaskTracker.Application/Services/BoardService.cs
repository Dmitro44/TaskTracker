using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Application.Services;

public class BoardService : IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly IGenericMapper<BoardDto, Board> _boardMapper;

    public BoardService(IBoardRepository boardRepository, IGenericMapper<BoardDto, Board> boardMapper)
    {
        _boardRepository = boardRepository;
        _boardMapper = boardMapper;
    }
    
    public async Task Create(BoardDto dto, CancellationToken ct)
    {
        var board = _boardMapper.ToEntity(dto);

        await _boardRepository.AddAsync(board, ct);
    }

    public async Task<IEnumerable<BoardDto>> GetBoards(Guid userId, CancellationToken ct)
    {
        var boards = await _boardRepository.GetAllByUserAsync(userId, ct);
        
        return boards.Select(board => _boardMapper.ToDto(board));
    }
}