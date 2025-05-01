using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.Contracts.Columns.Requests;
using TaskTracker.API.Contracts.Columns.Responses;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ColumnController : ControllerBase
{
    private readonly IColumnService _columnService;

    public ColumnController(IColumnService columnService)
    {
        _columnService = columnService;
    }
    
    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateColumnRequest request, CancellationToken ct)
    {
        var columnDto = new ColumnDto
        {
            Title = request.Title,
            Position = request.Position,
            BoardId = request.BoardId
        };

        await _columnService.Create(columnDto, ct);

        return Ok();
    }

    [HttpGet("GetColumns")]
    public async Task<IActionResult> GetColumns(Guid boardId, CancellationToken ct)
    {
        var columns = await _columnService.GetColumns(boardId, ct);

        return Ok(new ColumnListResponse(columns.ToList()));
    }
}