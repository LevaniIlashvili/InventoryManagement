using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.DTOs.Inventory;

public sealed record UpdateInventoryCustomFieldRequest(
    string Title,
    string Description,
    bool ShouldBeDisplayed,
    FieldType Type,
    int Order);