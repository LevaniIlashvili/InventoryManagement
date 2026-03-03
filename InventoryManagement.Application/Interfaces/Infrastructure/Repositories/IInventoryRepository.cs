using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces.Infrastructure.Repositories;

public interface IInventoryRepository
{
    Task<Guid> AddAsync(Inventory inventory);
    Task<Inventory?> GetByIdAsync(Guid id);
    void Update(Inventory inventory);
    void Delete(Inventory inventory);
}
