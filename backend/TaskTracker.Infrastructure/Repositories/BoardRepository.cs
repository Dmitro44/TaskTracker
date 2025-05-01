using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Infrastructure.Repositories;

public class BoardRepository : IBoardRepository
{
    private readonly AppDbContext _context;

    public BoardRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Board?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Boards
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task AddAsync(Board board, CancellationToken ct)
    {
        await _context.Boards.AddAsync(board, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Board>> GetAllByUserAsync(Guid userId, CancellationToken ct)
    {
        return await _context.Boards
            .AsNoTracking()
            .Where(b => b.OwnerId == userId)
            .ToListAsync(ct);
    }
}