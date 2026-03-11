namespace InventoryManagement.Application.DTOs.Inventory;

public sealed record InventoryDTO(
    Guid Id,
    string Title,
    string Description,
    Guid CategoryId,
    bool IsPublic);
