using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Contracts.Labels.Requests;

public record AddLabelRequest(
    [Required] Guid LabelId);