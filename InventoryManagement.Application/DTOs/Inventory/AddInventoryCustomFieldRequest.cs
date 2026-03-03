using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.DTOs.Inventory;

public sealed record AddInventoryCustomFieldRequest(
    string Title,
    string Description,
    bool ShouldBeDisplayed,
    FieldType Type,
    int Order);
