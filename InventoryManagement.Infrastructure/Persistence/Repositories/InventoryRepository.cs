using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InventoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Inventory?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Inventories
            .Include(i => i.Tags)
            .Include(i => i.CustomFields)
            .Include(i => i.CustomIdElements)
            .Include(i => i.AccessList)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Inventory>> GetByIdsAsync(List<Guid> ids)
    {
        return await _dbContext.Inventories
            .Include(i => i.Tags)
            .Include(i => i.CustomFields)
            .Where(i => ids.Contains(i.Id))
            .ToListAsync();
    }

    public async Task<Guid> AddAsync(Inventory inventory)
    {
        await _dbContext.Inventories.AddAsync(inventory);

        return inventory.Id;
    }

    public void Update(Inventory inventory)
    {
        _dbContext.Inventories.Update(inventory);
    }

    public void Delete(Inventory inventory)
    {
        _dbContext.Inventories.Remove(inventory);
    }

    public void DeleteRange(List<Inventory> inventories)
    {
        _dbContext.Inventories.RemoveRange(inventories);
    }
}
