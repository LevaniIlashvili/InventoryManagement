namespace InventoryManagement.Application.DTOs.Inventory;

public sealed record UpdateInventoryRequest(
    string Title,
    string Description,
    Guid CategoryId,
    string? ImageUrl,
    bool IsPublic,
    List<string> Tags);