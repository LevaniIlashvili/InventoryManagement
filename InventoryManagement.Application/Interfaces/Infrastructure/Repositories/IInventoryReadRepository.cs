using InventoryManagement.Application.DTOs.Inventory;

namespace InventoryManagement.Application.Interfaces.Infrastructure.Repositories;

public interface IInventoryReadRepository
{
    Task<List<InventoryDTO>> GetUserInventoriesAsync(Guid userId);
}
