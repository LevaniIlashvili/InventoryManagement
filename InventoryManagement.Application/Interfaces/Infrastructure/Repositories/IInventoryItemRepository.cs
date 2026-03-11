using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces.Infrastructure.Repositories;

public interface IInventoryItemRepository
{
    Task<InventoryItem?> GetByIdAsync(Guid id);
    Task<List<InventoryItem>> GetByInventoryId(Guid inventoryId);
    Task<Guid> AddItemAsync(InventoryItem item);
    void UpdateItem(InventoryItem item);
    void RemoveItem(InventoryItem item);
    Task<int> GetNextSequence(Guid inventoryId);
}
