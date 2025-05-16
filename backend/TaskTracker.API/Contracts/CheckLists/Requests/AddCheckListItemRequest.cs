using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Contracts.CheckLists.Requests;

public record AddCheckListItemRequest(
    [Required] string Text,
    [Required] int Position,
    [Required] string CheckListId);