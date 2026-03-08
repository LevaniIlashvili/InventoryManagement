namespace InventoryManagement.Application.DTOs.InventoryItem;

public sealed record AddInventoryItemRequest(Guid InventoryId, List<AddCustomFieldValueDTO> CustomFieldValues);

