using InventoryManagement.Application.DTOs.Inventory;

namespace InventoryManagement.Application.Interfaces.Application;

public interface IInventoryService
{
    Task<GetInventoryResponse> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(Guid userId, CreateInventoryRequest request);
    Task UpdateAsync(Guid userId, bool isAdmin, Guid inventoryId, UpdateInventoryRequest request);
    Task DeleteAsync(Guid userId, bool isAdmin, Guid inventoryId);
    Task<Guid> AddCustomFieldAsync(Guid userId, bool isAdmin, Guid inventoryId, AddInventoryCustomFieldRequest request);
    Task UpdateCustomFieldAsync(Guid userId, bool isAdmin, Guid inventoryId, Guid fieldId, UpdateInventoryCustomFieldRequest request);
    Task RemoveCustomFieldsAsync(
        Guid userId,
        bool isAdmin,
        Guid inventoryId,
        List<Guid> fieldIds);
}
