using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Interfaces;

public interface ICardService
{
    Task CreateCard(CardDto dto, CancellationToken ct);
    Task<IEnumerable<CardDto>> GetCards(Guid columnId, CancellationToken ct);
    Task<CardDto> UpdateCard(CardDto dto, CancellationToken ct);
}