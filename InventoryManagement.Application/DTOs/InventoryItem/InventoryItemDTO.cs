namespace InventoryManagement.Application.DTOs.InventoryItem;

public sealed record InventoryItemDTO(
    Guid Id,
    string CustomId,
    Guid InventoryId,
    Guid CreatedBy,
    DateTimeOffset CreatedAt,
    List<CustomFieldValueDTO> CustomFieldValues);
