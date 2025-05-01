using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Interfaces.Repositories;
using Card = TaskTracker.Domain.Entities.Card;

namespace TaskTracker.Application.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IGenericMapper<CardDto, Card> _cardMapper;
    
    public CardService(ICardRepository cardRepository, IGenericMapper<CardDto, Card> cardMapper)
    {
        _cardRepository = cardRepository;
        _cardMapper = cardMapper;
    }
    
    public async Task CreateCard(CardDto dto, CancellationToken ct)
    {
        var card = _cardMapper.ToEntity(dto);
        
        await _cardRepository.AddAsync(card, ct);
    }

    public Task CreateCard(Card dto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CardDto>> GetCards(Guid columnId, CancellationToken ct)
    {
        var cards = await _cardRepository.GetAllByColumnIdAsync(columnId, ct);
        
        return cards.Select(c => _cardMapper.ToDto(c));
    }

    public async Task<CardDto> UpdateCard(CardDto dto, CancellationToken ct)
    {
        var card = await _cardRepository.GetByIdAsync(dto.Id, ct);
        if (card is null) throw new InvalidOperationException($"Card with ID {dto.Id} not found");

        _cardMapper.MapPartial(dto, card);
        
        await _cardRepository.UpdateAsync(card, ct);
        
        return _cardMapper.ToDto(card);
    }
}