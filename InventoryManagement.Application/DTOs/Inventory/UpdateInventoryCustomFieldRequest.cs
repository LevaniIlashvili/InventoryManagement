using InventoryManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs.Inventory;

public sealed record UpdateInventoryCustomFieldRequest(
    [MinLength(3)]
    string Title,
    string Description,
    bool ShouldBeDisplayed,
    FieldType Type,
    int Order);