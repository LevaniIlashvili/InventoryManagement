namespace InventoryManagement.Application.DTOs.Inventory;

public sealed record GetInventoryResponse(
    Guid Id,
    string Title,
    string Description,
    Guid CreatedBy,
    Guid CategoryId,
    string? ImageUrl,
    bool IsPublic,
    List<InventoryTagDTO> Tags,
    List<InventoryCustomFieldDTO> CustomFields,
    List<CustomIdElementDTO> CustomIdElements);
