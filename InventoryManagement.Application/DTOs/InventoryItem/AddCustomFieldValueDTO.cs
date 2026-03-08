namespace InventoryManagement.Application.DTOs.InventoryItem;

public sealed record AddCustomFieldValueDTO(Guid InventoryCustomFieldId, string? Value);