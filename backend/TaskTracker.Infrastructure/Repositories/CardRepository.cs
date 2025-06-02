using Microsoft.EntityFrameworkCore;
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

    public async Task<IEnumerable<Card>> GetAllByColumnAsync(Guid columnId, CancellationToken ct)
    {
        return await _dbContext.Cards
            .AsNoTracking()
            .Where(c => c.ColumnId == columnId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetAllAsync(CancellationToken ct)
    {
        return await _dbContext.Cards
            .AsNoTracking()
            .Include(c => c.CardLabels)
            .ThenInclude(cl => cl.Label)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetCardsByColumns(IEnumerable<Guid> columnIds, CancellationToken ct)
    {
        return await _dbContext.Cards
            .Where(c => columnIds.Contains(c.ColumnId))
            .Include(c => c.CardLabels)
            .ThenInclude(cl => cl.Label)
            .ToListAsync(ct);
    }
}