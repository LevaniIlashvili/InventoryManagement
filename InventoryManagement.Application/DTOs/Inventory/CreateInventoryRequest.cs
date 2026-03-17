using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs.Inventory;

public sealed record CreateInventoryRequest(
    [MinLength(3)]
    string Title,
    [MinLength(3)]
    string Description,
    Guid CategoryId,
    string? ImageUrl,
    bool IsPublic,
    List<string> Tags);