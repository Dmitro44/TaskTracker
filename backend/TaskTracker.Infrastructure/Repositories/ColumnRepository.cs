using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Infrastructure.Repositories;

public class ColumnRepository : IColumnRepository
{
    private readonly AppDbContext _dbContext;

    public ColumnRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Column?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbContext.Columns
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(Column column, CancellationToken ct)
    {
        await _dbContext.Columns.AddAsync(column, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Column?>> GetAllByBoardAsync(Guid boardId, CancellationToken ct)
    {
        return await _dbContext.Columns
            .AsNoTracking()
            .Where(c => c.BoardId == boardId)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Column column, CancellationToken ct)
    {
        _dbContext.Columns.Update(column);
        await _dbContext.SaveChangesAsync(ct);
    }
}