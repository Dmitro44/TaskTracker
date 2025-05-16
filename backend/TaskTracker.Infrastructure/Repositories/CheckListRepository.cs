using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Infrastructure.Repositories;

public class CheckListRepository : ICheckListRepository
{
    private readonly AppDbContext _dbContext;

    public CheckListRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<CheckList?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbContext.CheckLists
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(CheckList checkList, CancellationToken ct)
    {
        await _dbContext.CheckLists.AddAsync(checkList, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task AddItemAsync(CheckListItem checkListItem, CancellationToken ct)
    {
        await _dbContext.CheckListItems.AddAsync(checkListItem, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<CheckListItem?> GetItemByIdAsync(Guid clItemId, CancellationToken ct)
    {
        return await _dbContext.CheckListItems
            .AsNoTracking()
            .FirstOrDefaultAsync(cli => cli.Id == clItemId, ct);
    }

    public async Task UpdateCheckListItemAsync(CheckListItem cliToUpdate, CancellationToken ct)
    {
        _dbContext.CheckListItems.Update(cliToUpdate); 
        await _dbContext.SaveChangesAsync(ct);
    }
}