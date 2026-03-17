using InventoryManagement.Application.DTOs.User;

namespace InventoryManagement.Application.DTOs.Inventory;

public record InventoryAccessDTO(Guid Id, Guid UserId, UserDTO User);
