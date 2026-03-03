namespace InventoryManagement.Application.DTOs.Inventory;

public sealed record CreateInventoryRequest(
    string Title,
    string Description,
    Guid CategoryId,
    string? ImageUrl,
    bool IsPublic,
    List<string> Tags);