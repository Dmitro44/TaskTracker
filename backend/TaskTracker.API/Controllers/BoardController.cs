using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.Contracts.Boards.Requests;
using TaskTracker.API.Contracts.Boards.Responses;
using TaskTracker.API.Extensions;
using TaskTracker.Application.DTOs.Board;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BoardController : ControllerBase
{
    private readonly IBoardService _boardService;

    public BoardController(IBoardService boardService)
    {
        _boardService = boardService;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateBoardRequest request, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();

        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }
        
        var boardDto = new BoardShortDto
        {
            Title = request.Title,
            IsPublic = request.IsPublic,
            OwnerId = userId 
        };
        
        await _boardService.Create(boardDto, ct);
        
        return Ok();
    }

    [HttpGet("getBoards")]
    public async Task<IActionResult> GetBoards(CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        var boards = await _boardService.GetBoards(userId, ct);

        return Ok(new BoardListResponse(boards.ToList()));
    }

    [HttpGet("{id:guid}/getBoard")]
    public async Task<IActionResult> GetBoard(Guid id, CancellationToken ct)
    {
        var board = await _boardService.GetBoardWithColumnsAndCards(id, ct);
        
        return Ok(board);
    }
}