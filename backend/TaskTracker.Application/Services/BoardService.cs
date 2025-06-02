using TaskTracker.Application.DTOs.Board;
using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.DTOs.Column;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Application.Services;

public class BoardService : IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly IGenericMapper<BoardShortDto, Board> _boardMapper;
    private readonly IColumnService _columnService;
    private readonly ICardService _cardService;
    private readonly IUserService _userService;

    public BoardService(
        IBoardRepository boardRepository,
        IGenericMapper<BoardShortDto, Board> boardMapper,
        IColumnService columnService,
        ICardService cardService,
        IUserService userService)
    {
        _boardRepository = boardRepository;
        _boardMapper = boardMapper;
        _columnService = columnService;
        _cardService = cardService;
        _userService = userService;
    }
    
    public async Task Create(BoardShortDto shortDto, CancellationToken ct)
    {
        var board = _boardMapper.ToEntity(shortDto);

        await _boardRepository.AddAsync(board, ct);
    }

    public async Task<IEnumerable<BoardShortDto>> GetBoards(Guid userId, CancellationToken ct)
    {
        var boards = await _boardRepository.GetAllByUserAsync(userId, ct);
        
        return boards.Select(board => _boardMapper.ToDto(board));
    }

    public async Task<BoardFullDto> GetBoardWithColumnsAndCards(Guid boardId, CancellationToken ct)
    {
        var board = await _boardRepository.GetByIdAsync(boardId, ct);
        
        if (board is null) throw new InvalidOperationException("Board not found");

        var columns = (await _columnService.GetColumns(boardId, ct)).ToList();
        var columnIds = columns.Select(c => c.Id).ToList();
        var cardsByColumns = await _cardService.GetCardsByColumns(columnIds, ct);

        var columnsFull = columns.Select(col => new ColumnFullDto
        {
            Id = col.Id,
            Title = col.Title ?? "",
            Position = col.Position!.Value,
            Cards = cardsByColumns.TryGetValue(col.Id, out var cards) ? cards : new List<CardDto>()
        });

        return new BoardFullDto
        {
            Id = board.Id,
            Title = board.Title,
            IsPublic = board.IsPublic,
            Columns = columnsFull.ToList()
        };
    }

    public async Task ArchiveBoard(Guid boardId, Guid userId, CancellationToken ct)
    {
        var board = await _boardRepository.GetByIdAsync(boardId, ct);
        if (board is null) 
            throw new InvalidOperationException($"Board to archive with ID {boardId} not found");
        
        var userDto = await _userService.GetById(userId, ct);
        
        board.IsArchived = true;
        board.ArchivedAt = DateTime.UtcNow;
        board.ArchivedBy = string.Join(" ", userDto.FirstName, userDto.LastName);

        await _boardRepository.UpdateAsync(board, ct);
    }

    public async Task RestoreBoard(Guid boardId, CancellationToken ct)
    {
        var board = await _boardRepository.GetByIdAsync(boardId, ct);
        if (board is null) 
            throw new InvalidOperationException($"Board to restore with ID {boardId} not found");
        
        board.IsArchived = false;
        board.ArchivedAt = null;
        board.ArchivedBy = null;
        
        await _boardRepository.UpdateAsync(board, ct);
    }
}