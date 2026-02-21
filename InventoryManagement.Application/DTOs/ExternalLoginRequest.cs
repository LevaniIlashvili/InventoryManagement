namespace InventoryManagement.Application.DTOs;

public class ExternalLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string Provider { get; set; } = string.Empty;
}
