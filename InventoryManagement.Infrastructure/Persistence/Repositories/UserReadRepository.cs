using InventoryManagement.Application.DTOs.User;
using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

public class UserReadRepository : IUserReadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserReadRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserDTO>> GetUsersAsync()
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Select(u => new UserDTO(
                u.Id,
                u.FirstName,
                u.LastName,
                u.UserName!,
                u.Email!,
                u.IsBlocked,
                u.CreatedAt,
                u.ProfilePictureUrl))
            .ToListAsync();
    }
}
