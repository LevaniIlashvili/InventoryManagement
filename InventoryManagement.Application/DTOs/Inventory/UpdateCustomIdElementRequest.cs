using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.DTOs.Inventory;

public sealed record UpdateCustomIdElementRequest(
    int Order,
    CustomIdElementType Type,
    string? FixedText,
    string? Format);
