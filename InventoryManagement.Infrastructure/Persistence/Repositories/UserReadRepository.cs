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

    public async Task<List<UserDTO>> SearchUsersAsync(string q)
    {
        var users = await _dbContext.Users
            .Where(u => u.UserName.Contains(q) || u.Email.Contains(q))
            .Take(10)
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

        return users;
    }

    public async Task<UserDTO?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users
        .AsNoTracking()
        .Where(u => u.Id == id.ToString())
        .Select(u => new UserDTO(
            u.Id,
            u.FirstName,
            u.LastName,
            u.UserName!,
            u.Email!,
            u.IsBlocked,
            u.CreatedAt,
            u.ProfilePictureUrl))
        .FirstOrDefaultAsync();
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
