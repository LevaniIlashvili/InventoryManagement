using InventoryManagement.Application.DTOs.User;

namespace InventoryManagement.Application.Interfaces.Infrastructure.Repositories;

public interface IUserReadRepository
{
    Task<List<UserDTO>> GetUsersAsync();
}
