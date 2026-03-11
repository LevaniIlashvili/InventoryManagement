using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.DTOs.Inventory;

public record CustomIdElementDTO(
    Guid Id,
    int Order,
    CustomIdElementType Type,
    string? FixedText,
    string? Format);
