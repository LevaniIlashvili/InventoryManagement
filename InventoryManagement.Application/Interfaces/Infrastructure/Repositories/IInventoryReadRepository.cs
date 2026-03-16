using InventoryManagement.Application.DTOs.Inventory;

namespace InventoryManagement.Application.Interfaces.Infrastructure.Repositories;

public interface IInventoryReadRepository
{
    Task<List<InventoryDTO>> GetInventoriesByTagAsync(string tag);
    Task<List<InventoryDTO>> SearchInventoriesAsync(string searchTerm);
    Task<GetInventoryStatisticsResponse> GetInventoryStatisticsAsync(Guid inventoryId);
    Task<List<InventoryDTO>> GetUserInventoriesAsync(Guid userId);
    Task<List<InventoryDTO>> GetPopularInventoriesAsync();
    Task<List<InventoryDTO>> GetLatestInventoriesAsync();
}
