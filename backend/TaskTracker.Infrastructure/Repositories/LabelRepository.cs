using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Infrastructure.Repositories;

public class LabelRepository : ILabelRepository
{
    private readonly AppDbContext _dbContext;

    public LabelRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Label?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbContext.Labels
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, ct);
    }

    public async Task AddAsync(Label label, CancellationToken ct)
    {
        await _dbContext.Labels.AddAsync(label, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Label>> GetAllByBoardAsync(Guid boardId, CancellationToken ct)
    {
        return await _dbContext.Labels
            .AsNoTracking()
            .Where(l => l.BoardId == boardId)
            .ToListAsync(ct);
    }

    public async Task AttachLabelToCardAsync(Guid cardId, Guid labelId, CancellationToken ct)
    {
        var exists = await  _dbContext.CardLabels
            .AnyAsync(cl => cl.CardId == cardId && cl.LabelId == labelId, ct);
        
        if (exists)
            throw new InvalidOperationException("Card label already exists");

        var cardLabel = new CardLabel
        {
            CardId = cardId,
            LabelId = labelId
        };
        
        await _dbContext.CardLabels.AddAsync(cardLabel, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task RemoveLabelFromCardAsync(Guid cardId, Guid labelId, CancellationToken ct)
    {
        var cardLabel = await _dbContext.CardLabels
            .FirstOrDefaultAsync(cl => cl.CardId == cardId && cl.LabelId == labelId, ct);

        if (cardLabel != null)
        {
            _dbContext.CardLabels.Remove(cardLabel);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}