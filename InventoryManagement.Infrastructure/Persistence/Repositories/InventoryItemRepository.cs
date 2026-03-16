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
        return _context.InventoryItems
            .OrderByDescending(i => i.CreatedAt)
            .Include(i => i.Values)
            .Where(i => i.InventoryId == inventoryId)
            .ToListAsync();
    }

    public void RemoveItem(InventoryItem item)
    {
        _context.Remove(item);
    }

    public void UpdateItem(InventoryItem item)
    {
        _context.Update(item);
    }

    public async Task<int> GetNextSequence(Guid inventoryId)
    {
        int maxRetries = 5;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Inventory not found.");

            int allocatedSequence = inventory.CurrentSequence;
            inventory.CurrentSequence++;

            try
            {
                await _context.SaveChangesAsync();

                return allocatedSequence;
            }
            catch (DbUpdateConcurrencyException)
            {
                _context.Entry(inventory).State = EntityState.Detached;

                if (attempt == maxRetries - 1)
                    throw new Exception("High concurrency prevented sequence generation. Please try again.");
            }
        }

        throw new Exception("Failed to generate sequence.");
    }
}
