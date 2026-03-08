namespace InventoryManagement.Application.Interfaces.Application;

public interface IUserService
{
    Task DeleteUserAsync(string id);
    Task BlockUserAsync(string id);
    Task UnblockUserAsync(string id);
    Task GrantRoleAsync(string userId, string role);
    Task RevokeRoleAsync(string userId, string role);
}
