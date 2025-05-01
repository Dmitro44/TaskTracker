using TaskTracker.Application.DTOs;

namespace TaskTracker.API.Contracts.Boards.Responses;

public record BoardListResponse(
    List<BoardDto> Boards);