using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.DTOs.Inventory;

public sealed record InventoryCustomFieldDTO(
    Guid Id,
    Guid InventoryId,
    string Title,
    string Description,
    bool ShouldBeDisplayed,
    FieldType Type,
    int Order);