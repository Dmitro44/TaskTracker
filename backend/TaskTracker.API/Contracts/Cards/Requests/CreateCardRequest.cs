using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Contracts.Cards.Requests;

public record CreateCardRequest(
    [Required] string Title,
    [Required] int Position,
    [Required] Guid ColumnId);