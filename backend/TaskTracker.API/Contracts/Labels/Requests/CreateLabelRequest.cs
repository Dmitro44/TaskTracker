using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Contracts.Labels.Requests;

public record CreateLabelRequest(
    [Required] string Name,
    [Required] string Color,
    [Required] Guid BoardId);