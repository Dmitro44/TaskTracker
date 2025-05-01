using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Contracts.Cards.Requests;

public record UpdateCardRequest(
    [Required] string Title,
    [Required] int Position,
    [Required] Guid ColumnId);