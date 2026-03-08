using InventoryManagement.Application.DTOs.InventoryItem;
using InventoryManagement.Application.Interfaces.Application;
using MediatR;
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

        var id = await _inventoryItemService.AddItemAsync(Guid.Parse(userId), request);

        return Ok(id);
    }

    [HttpPut("{itemId}")]
    [Authorize]
    public async Task<IActionResult> UpdateItem([FromRoute] Guid itemId, [FromBody] List<AddCustomFieldValueDTO> customFields)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _inventoryItemService.UpdateItemAsync(Guid.Parse(userId), itemId, customFields);

        return NoContent();
    }

    [HttpDelete("{itemId}")]
    [Authorize]
    public async Task<IActionResult> RemoveItem([FromRoute] Guid itemId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _inventoryItemService.RemoveItemAsync(Guid.Parse(userId), itemId);

        return NoContent();
    }
}
