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
}