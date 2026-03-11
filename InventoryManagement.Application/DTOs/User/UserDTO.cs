namespace InventoryManagement.Application.DTOs.User;

public sealed record UserDTO(
    string Id,
    string? FirstName,
    string? LastName,
    string Username,
    string Email,
    bool IsBlocked,
    DateTimeOffset CreatedAt,
    string? ProfileUrl
);