using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.Contracts.Cards.Requests;
using TaskTracker.API.Contracts.Cards.Responses;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardController(ICardService cardService)
    {
        _cardService = cardService;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateCardRequest request, CancellationToken ct)
    {
        var cardDto = new CardDto
        {
            Title = request.Title,
            Position = request.Position,
            ColumnId = request.ColumnId
        };

        await _cardService.CreateCard(cardDto, ct);

        return Ok();
    }

    [HttpPatch("{id:guid}/Move")]
    public async Task<IActionResult> Move(Guid id, [FromBody] MoveCardRequest request, CancellationToken ct)
    {
        var cardDto = new CardDto
        {
            Id = id,
            Position = request.Position,
            ColumnId = request.ToColumnId
        };

        var updatedCard = await _cardService.UpdateCard(cardDto, ct);
        
        return Ok(new MoveCardResponse(updatedCard.Position!.Value, updatedCard.ColumnId!.Value));
    }
}