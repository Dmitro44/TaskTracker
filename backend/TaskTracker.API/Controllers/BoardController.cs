using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.Contracts.Boards.Requests;
using TaskTracker.API.Contracts.Boards.Responses;
using TaskTracker.API.Extensions;
using TaskTracker.Application.DTOs;
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
    
    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateBoardRequest request, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();

        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }
        
        var boardDto = new BoardDto
        {
            Title = request.Title,
            IsPublic = request.IsPublic,
            OwnerId = userId 
        };
        
        await _boardService.Create(boardDto, ct);
        
        return Ok();
    }

    [HttpGet("GetBoards")]
    public async Task<IActionResult> GetBoards(CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        var boards = await _boardService.GetBoards(userId, ct);

        return Ok(new BoardListResponse(boards.ToList()));
    }
}