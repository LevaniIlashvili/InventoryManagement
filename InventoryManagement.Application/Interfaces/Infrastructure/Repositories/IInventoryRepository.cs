using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces.Infrastructure.Repositories;

public interface IInventoryRepository
{
    Task<Guid> AddAsync(Inventory inventory);
    Task<Inventory?> GetByIdAsync(Guid id);
    Task<List<Inventory>> GetByIdsAsync(List<Guid> ids);
    void Update(Inventory inventory);
    void Delete(Inventory inventory);
    void DeleteRange(List<Inventory> inventories);
}
