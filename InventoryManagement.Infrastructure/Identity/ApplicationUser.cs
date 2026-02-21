using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Infrastructure;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsBlocked { get; set; } = false;
    public string? ProfilePictureUrl { get; set; }
}
