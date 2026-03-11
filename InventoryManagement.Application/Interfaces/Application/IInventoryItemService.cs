using InventoryManagement.Application.DTOs.InventoryItem;

namespace InventoryManagement.Application.Interfaces.Application;

public interface IInventoryItemService
{
    Task<List<InventoryItemDTO>> GetItemsAsync(Guid inventoryId);
    Task<Guid> AddItemAsync(Guid userId, AddInventoryItemRequest request);
    Task UpdateItemAsync(Guid userId, Guid itemId, List<AddCustomFieldValueDTO> customFields);
    Task RemoveItemsAsync(Guid userId, List<Guid> itemIds);
}
