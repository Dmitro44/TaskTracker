using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.Contracts.Cards.Requests;
using TaskTracker.API.Contracts.Cards.Responses;
using TaskTracker.API.Contracts.CheckLists.Requests;
using TaskTracker.API.Contracts.Labels.Requests;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;
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

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateCardRequest request, CancellationToken ct)
    {
        var cardDto = new CardDto
        {
            Title = request.Title,
            Position = request.Position,
            ColumnId = request.ColumnId
        };

        var card = await _cardService.CreateCard(cardDto, ct);

        return Ok(card);
    }

    [HttpPatch("{id:guid}/move")]
    public async Task<IActionResult> Move(Guid id, [FromBody] MoveCardRequest request, CancellationToken ct)
    {
        var cardDto = new CardDto
        {
            Id = id,
            Position = request.Position,
            ColumnId = request.ToColumnId
        };

        var updatedCard = await _cardService.MoveCards(cardDto, ct);
        
        return Ok(new MoveCardResponse(updatedCard.Position!.Value, updatedCard.ColumnId!.Value));
    }

    [HttpGet("{boardId:guid}/getCards")]
    public async Task<IActionResult> GetCards(Guid boardId, CancellationToken ct)
    {
        var cards = await _cardService.GetAllCards(boardId, ct);

        return Ok(cards.ToList());
    }

    [HttpPost("{cardId:guid}/addLabel")]
    public async Task<IActionResult> AddLabel(Guid cardId, [FromBody] Guid labelId, CancellationToken ct)
    {
        await _cardService.AddLabelToCard(cardId, labelId, ct);
        return Ok();
    }

    [HttpDelete("{cardId:guid}/removeLabel/{labelId:guid}")]
    public async Task<IActionResult> RemoveLabel(Guid cardId, Guid labelId, CancellationToken ct)
    {
        await _cardService.RemoveLabelFromCard(cardId, labelId, ct);
        return Ok();
    }

    [HttpPost("{cardId:guid}/addCheckList")]
    public async Task<IActionResult> AddCheckList(Guid cardId, [FromBody] string title, CancellationToken ct)
    {
        var checkListDto = new CheckListDto
        {
            Title = title,
            CardId = cardId
        };
        
        await _cardService.AddCheckList(checkListDto, ct);
        return Ok();
    }

    
    [HttpPost("card/cl/{clId:guid}/addCheckListItem")]
    public async Task<IActionResult> AddCheckListItem(Guid clId, [FromBody] AddCheckListItemRequest request, CancellationToken ct)
    {
        var checkListItemDto = new CheckListItemDto
        {
            Text = request.Text,
            Position = request.Position,
            CheckListId = clId
        };
        
        await _cardService.AddCheckListItem(checkListItemDto, ct);

        return Ok();
    }

    [HttpPost("card/cl/{clId:guid}/completeCheckListItem/{clItemId:guid}")]
    public async Task<IActionResult> CompleteCheckListItem(Guid clItemId, CancellationToken ct)
    {
        var checkListItemDto = new CheckListItemDto
        {
            IsCompleted = true
        };
        
        var updatedClItem =  await _cardService.UpdateCheckListItem(clItemId, checkListItemDto, ct);

        return Ok(updatedClItem);
    }
}