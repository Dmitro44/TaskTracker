using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Contracts.Boards.Requests;

public record CreateBoardRequest(
    [Required] string Title,
    [Required] bool IsPublic);