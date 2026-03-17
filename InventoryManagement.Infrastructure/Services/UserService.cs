using InventoryManagement.Application.DTOs.User;
using InventoryManagement.Application.Exceptionsl;
using InventoryManagement.Application.Interfaces.Application;
using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserReadRepository _userReadRepository;

    public UserService(UserManager<ApplicationUser> userManager, IUserReadRepository userReadRepository)
    {
        _userManager = userManager;
        _userReadRepository = userReadRepository;
    }

    public async Task<List<UserDTO>> SearchUsersAsync(string q)
    {
        return await _userReadRepository.SearchUsersAsync(q);
    }

    public async Task<List<UserDTO>> GetUsersAsync()
    {
        return await _userReadRepository.GetUsersAsync();
    }

    public async Task DeleteUsersAsync(List<string> ids)
    {
        foreach (var id in ids)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User {id} not found");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(',', result.Errors));
        }
    }

    public async Task BlockUsersAsync(List<string> ids)
    {
        foreach (var id in ids)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User {id} not found");

            user.IsBlocked = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(',', result.Errors));
        }
    }
    public async Task UnblockUsersAsync(List<string> ids)
    {
        foreach (var id in ids)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User {id} not found");

            user.IsBlocked = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(',', result.Errors));
        }
    }

    public async Task GrantRolesAsync(List<string> ids, string role)
    {
        foreach (var id in ids)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User {id} not found");

            var result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(',', result.Errors));
        }
    }

    public async Task RevokeRolesAsync(List<string> ids, string role)
    {
        foreach (var id in ids)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User {id} not found");

            var result = await _userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(',', result.Errors));
        }
    }
}
