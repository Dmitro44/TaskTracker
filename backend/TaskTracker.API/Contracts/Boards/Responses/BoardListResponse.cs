using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Board;

namespace TaskTracker.API.Contracts.Boards.Responses;

public record BoardListResponse(
    List<BoardShortDto> Boards);