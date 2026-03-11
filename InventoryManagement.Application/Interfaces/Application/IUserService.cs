using InventoryManagement.Application.DTOs.User;

namespace InventoryManagement.Application.Interfaces.Application;

public interface IUserService
{
    Task<List<UserDTO>> GetUsersAsync();
    Task DeleteUsersAsync(List<string> ids);
    Task BlockUsersAsync(List<string> ids);
    Task UnblockUsersAsync(List<string> ids);
    Task GrantRolesAsync(List<string> ids, string role);
    Task RevokeRolesAsync(List<string> ids, string role);
}
