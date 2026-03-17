using InventoryManagement.Application.DTOs.User;

namespace InventoryManagement.Application.Interfaces.Infrastructure.Repositories;

public interface IUserReadRepository
{
    Task<List<UserDTO>> SearchUsersAsync(string q);
    Task<UserDTO?> GetByIdAsync(Guid id);
    Task<List<UserDTO>> GetUsersAsync();
}
