using InventoryManagement.Application.DTOs.InventoryItem;
using InventoryManagement.Application.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InventoryItemController : ControllerBase
{
    private readonly IInventoryItemService _inventoryItemService;

    public InventoryItemController(IInventoryItemService inventoryItemService)
    {
        _inventoryItemService = inventoryItemService;
    }

    [HttpGet("/api/inventory/{inventoryId}/items")]
    public async Task<ActionResult<List<InventoryItemDTO>>> GetItems([FromRoute] Guid inventoryId)
    {
        var items = await _inventoryItemService.GetItemsAsync(inventoryId);

        return Ok(items);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> AddItem([FromBody] AddInventoryItemRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var id = await _inventoryItemService.AddItemAsync(Guid.Parse(userId), isAdmin, request);

        return Ok(id);
    }

    [HttpPut("{itemId}")]
    [Authorize]
    public async Task<IActionResult> UpdateItem([FromRoute] Guid itemId, [FromBody] List<AddCustomFieldValueDTO> customFields)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        await _inventoryItemService.UpdateItemAsync(Guid.Parse(userId), isAdmin, itemId, customFields);

        return NoContent();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> RemoveItems([FromBody] List<Guid> itemIds)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        await _inventoryItemService.RemoveItemsAsync(userId, isAdmin, itemIds);

        return NoContent();
    }
}
