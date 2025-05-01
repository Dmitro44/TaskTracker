using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Contracts.Columns.Requests;

public record CreateColumnRequest(
    [Required] string Title,
    [Required] int Position,
    [Required] Guid BoardId);