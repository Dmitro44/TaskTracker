using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Column;

namespace TaskTracker.API.Contracts.Columns.Responses;

public record ColumnListResponse(
    List<ColumnShortDto> Columns);