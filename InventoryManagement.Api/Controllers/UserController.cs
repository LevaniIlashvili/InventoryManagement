using InventoryManagement.Application.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
    {
        await _userService.DeleteUserAsync(id);

        return NoContent();
    }

    [HttpPost("{id}/block")]
    public async Task<IActionResult> BlockUser([FromRoute] string id)
    {
        await _userService.BlockUserAsync(id);

        return NoContent();
    }

    [HttpPost("{id}/unblock")]
    public async Task<IActionResult> UnblockUser([FromRoute] string id)
    {
        await _userService.UnblockUserAsync(id);

        return NoContent();
    }

    [HttpPost("{id}/grant-admin")]
    public async Task<IActionResult> GrantAdmin([FromRoute] string id)
    {
        await _userService.GrantRoleAsync(id, "Admin");

        return NoContent();
    }

    [HttpPost("{id}/revoke-admin")]
    public async Task<IActionResult> RevokeAdmin([FromRoute] string id)
    {
        await _userService.RevokeRoleAsync(id, "Admin");

        return NoContent();
    }
}