using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.Contracts.Columns.Requests;
using TaskTracker.API.Contracts.Columns.Responses;
using TaskTracker.Application.DTOs.Column;
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
    
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateColumnRequest request, CancellationToken ct)
    {
        var columnDto = new ColumnShortDto
        {
            Title = request.Title,
            Position = request.Position,
            BoardId = request.BoardId
        };

        var column = await _columnService.Create(columnDto, ct);

        return Ok(column);
    }

    [HttpGet("getColumns")]
    public async Task<IActionResult> GetColumns([FromQuery] Guid boardId, CancellationToken ct)
    {
        var columns = await _columnService.GetColumns(boardId, ct);

        return Ok(new ColumnListResponse(columns.ToList()));
    }

    [HttpPatch("{id:guid}/move")]
    public async Task<IActionResult> Move(Guid id, [FromBody] int position, CancellationToken ct)
    {
        var columnDto = new ColumnShortDto
        {
            Id = id,
            Position = position
        };
        
        var updatedColumn = await _columnService.MoveColumns(columnDto, ct);
        
        return Ok(updatedColumn.Position!.Value);
    }

    [HttpPut("{id:guid}/update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateColumnRequest request, CancellationToken ct)
    {
        var updateColumnDto = new ColumnShortDto
        {
            Id = id,
            Title = request.Title,
        };
        
        await _columnService.UpdateColumn(updateColumnDto, ct);

        return Ok();
    }
}