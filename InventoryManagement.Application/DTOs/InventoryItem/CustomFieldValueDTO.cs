namespace InventoryManagement.Application.DTOs.InventoryItem;

public sealed record CustomFieldValueDTO(Guid Id, Guid InventoryCustomFieldId, string? Value);
