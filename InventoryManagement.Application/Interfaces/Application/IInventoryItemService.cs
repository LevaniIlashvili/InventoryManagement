using InventoryManagement.Application.DTOs.InventoryItem;

namespace InventoryManagement.Application.Interfaces.Application;

public interface IInventoryItemService
{
    Task<List<InventoryItemDTO>> GetItemsAsync(Guid inventoryId);
    Task<Guid> AddItemAsync(Guid userId, bool isAdmin, AddInventoryItemRequest request);
    Task UpdateItemAsync(Guid userId, bool isAdmin, Guid itemId, List<AddCustomFieldValueDTO> customFields);
    Task RemoveItemsAsync(Guid userId, bool isAdmin, List<Guid> itemIds);
}
