using TaskTracker.Application.DTOs;

namespace TaskTracker.API.Contracts.Columns.Responses;

public record ColumnListResponse(
    List<ColumnDto> Columns);