using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.Contracts.Labels.Requests;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LabelController : ControllerBase
{
    private readonly ILabelService _labelService;

    public LabelController(ILabelService labelService)
    {
        _labelService = labelService;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateLabelRequest request, CancellationToken ct)
    {
        var labelDto = new LabelDto
        {
            Name = request.Name,
            Color = request.Color,
            BoardId = request.BoardId
        };
        
        await _labelService.Create(labelDto, ct);
        
        return Ok();
    }

    [HttpGet("getLabels")]
    public async Task<IActionResult> GetLabels([FromQuery] Guid boardId, CancellationToken ct)
    {
        var labels = await _labelService.GetLabels(boardId, ct);

        return Ok(labels.ToList());
    }
}