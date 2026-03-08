using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

public class InventoryItemRepository : IInventoryItemRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddItemAsync(InventoryItem item)
    {
        await _context.InventoryItems.AddAsync(item);

        return item.Id;
    }

    public Task<InventoryItem?> GetByIdAsync(Guid id)
    {
        return _context.InventoryItems.Include(i => i.Values).FirstOrDefaultAsync(i => i.Id == id);
    }

    public Task<List<InventoryItem>> GetByInventoryId(Guid inventoryId)
    {
        return _context.InventoryItems.Include(i => i.Values).Where(i => i.InventoryId == inventoryId).ToListAsync();
    }

    public void RemoveItem(InventoryItem item)
    {
        _context.Remove(item);
    }

    public void UpdateItem(InventoryItem item)
    {
        _context.Update(item);
    }
}
