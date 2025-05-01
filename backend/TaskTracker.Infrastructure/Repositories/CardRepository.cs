using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;
using Card = TaskTracker.Domain.Entities.Card;

namespace TaskTracker.Infrastructure.Repositories;

public class CardRepository : ICardRepository
{
    private readonly AppDbContext _dbContext;

    public CardRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Card?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbContext.Cards
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(Card card, CancellationToken ct)
    {
        await _dbContext.Cards.AddAsync(card, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Card card, CancellationToken ct)
    {
        _dbContext.Cards.Update(card);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetAllByColumnIdAsync(Guid columnId, CancellationToken ct)
    {
        return await _dbContext.Cards
            .AsNoTracking()
            .Where(c => c.ColumnId == columnId)
            .ToListAsync(ct);
    }
}