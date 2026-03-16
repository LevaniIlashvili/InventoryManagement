using InventoryManagement.Application.DTOs.Inventory;

namespace InventoryManagement.Application.Interfaces.Application;

public interface IInventoryService
{
    Task<List<InventoryDTO>> GetInventoriesByTagAsync(string tag);
    Task<List<InventoryDTO>> SearchInventoriesAsync(string searchTerm);
    Task<GetInventoryStatisticsResponse> GetInventoryStatisticsAsync(Guid inventoryId);
    Task<List<InventoryDTO>> GetPopularInventoriesAsync();
    Task<List<InventoryDTO>> GetLatestInventoriesAsync();
    Task<List<InventoryDTO>> GetUserInventoriesAsync(Guid userId);
    Task<GetInventoryResponse> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(Guid userId, CreateInventoryRequest request);
    Task UpdateAsync(Guid userId, bool isAdmin, Guid inventoryId, UpdateInventoryRequest request);
    Task DeleteAsync(Guid userId, bool isAdmin, List<Guid> ids);
    Task<Guid> AddCustomFieldAsync(Guid userId, bool isAdmin, Guid inventoryId, AddInventoryCustomFieldRequest request);
    Task UpdateCustomFieldAsync(Guid userId, bool isAdmin, Guid inventoryId, Guid fieldId, UpdateInventoryCustomFieldRequest request);
    Task RemoveCustomFieldsAsync(
        Guid userId,
        bool isAdmin,
        Guid inventoryId,
        List<Guid> fieldIds);
    Task<Guid> AddCustomIdElementAsync(
        Guid userId,
        bool isAdmin,
        Guid inventoryId,
        AddCustomIdElementRequest request);

    Task UpdateCustomIdElementAsync(
        Guid userId,
        bool isAdmin,
        Guid inventoryId,
        Guid elementId,
        UpdateCustomIdElementRequest request);

    Task RemoveCustomIdElementsAsync(
        Guid userId,
        bool isAdmin,
        Guid inventoryId,
        List<Guid> elementIds);
}
