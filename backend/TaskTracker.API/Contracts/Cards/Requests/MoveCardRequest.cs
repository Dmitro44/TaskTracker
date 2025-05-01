using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Contracts.Cards.Requests;

public record MoveCardRequest(
    [Required] int Position,
    [Required] Guid ToColumnId);