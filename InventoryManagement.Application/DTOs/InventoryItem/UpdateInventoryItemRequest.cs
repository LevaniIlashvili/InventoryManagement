namespace InventoryManagement.Application.DTOs.InventoryItem;

public sealed record UpdateInventoryItemRequest(Guid ItemId, List<AddCustomFieldValueDTO> CustomFieldValues);
