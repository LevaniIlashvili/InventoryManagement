using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

public class InventoryTagRepository : IInventoryTagRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InventoryTagRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRangeAsync(List<InventoryTag> tags)
    {
        await _dbContext.AddRangeAsync(tags);
    }

    public async Task<List<InventoryTag>> GetAllAsync()
    {
        return await _dbContext.InventoryTags.ToListAsync();
    }

    public async Task<List<InventoryTag>> GetByNamesAsync(IEnumerable<string> names)
    {
        return await _dbContext.InventoryTags
            .Where(t => names.Contains(t.Name))
            .ToListAsync();
    }
}
