using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Contracts.Cards.Requests;

public record EditCardRequest(
    [Required] string Title,
    [Required] int Position,
    [Required] Guid ColumnId);