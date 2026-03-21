using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("search")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> SearchUsers([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Ok(new List<UserDto>());

        var users = await _userService.SearchUsersAsync(q);

        return Ok(users);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsersAsync();

        return Ok(users);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUsers([FromBody] List<string> ids)
    {
        await _userService.DeleteUsersAsync(ids);

        return NoContent();
    }

    [HttpPost("block")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BlockUsers([FromBody] List<string> ids)
    {
        await _userService.BlockUsersAsync(ids);

        return NoContent();
    }

    [HttpPost("unblock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UnblockUsers([FromBody] List<string> ids)
    {
        await _userService.UnblockUsersAsync(ids);

        return NoContent();
    }

    [HttpPost("grant-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GrantAdmins([FromBody] List<string> ids)
    {
        await _userService.GrantRolesAsync(ids, "Admin");

        return NoContent();
    }

    [HttpPost("revoke-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RevokeAdmins([FromBody] List<string> ids)
    {
        await _userService.RevokeRolesAsync(ids, "Admin");

        return NoContent();
    }
}