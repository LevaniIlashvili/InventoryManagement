using InventoryManagement.Application.Exceptionsl;
using InventoryManagement.Application.Interfaces.Application;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException();
    }

    public async Task BlockUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        user.IsBlocked = true;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(',', result.Errors));
    }

    public async Task UnblockUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        user.IsBlocked = false;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(',', result.Errors));
    }

    public async Task GrantRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(',', result.Errors));
    }

    public async Task RevokeRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, role);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(',', result.Errors));
    }
}
