using InventoryManagement.Application.DTOs.Inventory;
using InventoryManagement.Application.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("by-tag")]
    public async Task<ActionResult<List<InventoryDTO>>> GetByTag([FromQuery] string tag)
    {
        var results = await _inventoryService.GetInventoriesByTagAsync(tag);
        return Ok(results);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<InventoryDTO>>> Search([FromQuery] string q)
    {
        var results = await _inventoryService.SearchInventoriesAsync(q);
        return Ok(results);
    }

    [HttpGet("{id}/statistics")]
    public async Task<ActionResult<GetInventoryStatisticsResponse>> GetInventoryStatistics([FromRoute] Guid id)
    {
        var inventoryStatistics = await _inventoryService.GetInventoryStatisticsAsync(id);

        return Ok(inventoryStatistics);
    }
     
    [HttpGet("latest")]
    public async Task<ActionResult<List<InventoryDTO>>> GetLatestInventories()
    {
        var inventories = await _inventoryService.GetLatestInventoriesAsync();

        return Ok(inventories);
    }

    [HttpGet("popular")]
    public async Task<ActionResult<List<InventoryDTO>>> GetPopularInventories()
    {
        var inventories = await _inventoryService.GetPopularInventoriesAsync();

        return Ok(inventories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetInventoryResponse>> GetInventory([FromRoute] Guid id)
    {
        var inventory = await _inventoryService.GetByIdAsync(id);

        return Ok(inventory);
    }

    [HttpPost]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<Guid>> CreateInventory([FromBody] CreateInventoryRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var id = await _inventoryService.CreateAsync(Guid.Parse(userId), request);

        return Ok(id);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> UpdateInventory([FromRoute] Guid id, [FromBody] UpdateInventoryRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        await _inventoryService.UpdateAsync(userId, isAdmin, id, request);

        return NoContent();
    }

    [HttpDelete]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> DeleteInventories([FromBody] List<Guid> ids)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        await _inventoryService.DeleteAsync(userId, isAdmin, ids);

        return NoContent();
    }

    [HttpPost("{inventoryId}/custom-fields")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<Guid>> AddCustomField(
        [FromRoute] Guid inventoryId,
        [FromBody] AddInventoryCustomFieldRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        var id = await _inventoryService.AddCustomFieldAsync(
            userId,
            isAdmin,
            inventoryId,
            request);

        return Ok(id);
    }

    [HttpPut("{inventoryId}/custom-fields/{fieldId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> UpdateCustomField(
        [FromRoute] Guid inventoryId,
        [FromRoute] Guid fieldId,
        [FromBody] UpdateInventoryCustomFieldRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        await _inventoryService.UpdateCustomFieldAsync(
            userId,
            isAdmin,
            inventoryId,
            fieldId,
            request);

        return NoContent();
    }

    [HttpDelete("{inventoryId}/custom-fields")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> DeleteCustomFields(
        [FromRoute] Guid inventoryId,
        [FromBody] List<Guid> ids)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        await _inventoryService.RemoveCustomFieldsAsync(
            userId,
            isAdmin,
            inventoryId,
            ids);

        return NoContent();
    }

    [HttpPost("{inventoryId}/custom-id-elements")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<Guid>> AddCustomIdElement(
        [FromRoute] Guid inventoryId,
        [FromBody] AddCustomIdElementRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        var id = await _inventoryService.AddCustomIdElementAsync(
            userId,
            isAdmin,
            inventoryId,
            request);

        return Ok(id);
    }

    [HttpPut("{inventoryId}/custom-id-elements/{elementId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> UpdateCustomIdElement(
        [FromRoute] Guid inventoryId,
        [FromRoute] Guid elementId,
        [FromBody] UpdateCustomIdElementRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        await _inventoryService.UpdateCustomIdElementAsync(
            userId,
            isAdmin,
            inventoryId,
            elementId,
            request);

        return NoContent();
    }

    [HttpDelete("{inventoryId}/custom-id-elements")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> DeleteCustomIdElements(
        [FromRoute] Guid inventoryId,
        [FromBody] List<Guid> ids)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        await _inventoryService.RemoveCustomIdElementsAsync(
            userId,
            isAdmin,
            inventoryId,
            ids);

        return NoContent();
    }

    [Authorize]
    [HttpGet("/api/user/inventories")]
    public async Task<ActionResult<List<InventoryDTO>>> GetUserInventories()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var inventories = await _inventoryService.GetUserInventoriesAsync(userId);

        return Ok(inventories);
    }
}
