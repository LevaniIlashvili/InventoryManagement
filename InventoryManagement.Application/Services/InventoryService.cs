using InventoryManagement.Application.DTOs.Inventory;
using InventoryManagement.Application.Exceptions;
using InventoryManagement.Application.Exceptionsl;
using InventoryManagement.Application.Interfaces.Application;
using InventoryManagement.Application.Interfaces.Infrastructure;
using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IInventoryTagRepository _inventoryTagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(
        IInventoryRepository inventoryRepository, 
        IInventoryTagRepository inventoryTagRepository,
        IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _inventoryTagRepository = inventoryTagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetInventoryResponse> GetByIdAsync(Guid id)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);

        if (inventory == null)
        {
            throw new NotFoundException("Inventory not found");
        }

        return new GetInventoryResponse(
            inventory.Id,
            inventory.Title,
            inventory.Description,
            inventory.CreatedBy,
            inventory.CategoryId,
            inventory.ImageUrl,
            inventory.IsPublic,
            inventory.Tags.Select(t => new InventoryTagDTO(t.Id, t.Name)).ToList(),
            inventory.CustomFields.Select(f => new InventoryCustomFieldDTO(
                f.Id,
                f.InventoryId,
                f.Title,
                f.Description,
                f.ShouldBeDisplayed,
                f.Type,
                f.Order)).ToList());
    }

    public async Task<Guid> CreateAsync(Guid userId, CreateInventoryRequest request)
    {
        var inventory = new Inventory(
            Guid.NewGuid(),
            request.Title,
            request.Description,
            userId,
            request.CategoryId,
            request.ImageUrl,
            request.IsPublic);

        var tags = await ResolveTagsAsync(request.Tags);

        inventory.SetTags(tags);

        await _inventoryRepository.AddAsync(inventory);

        await _unitOfWork.SaveChangesAsync();

        return inventory.Id;
    }

    public async Task UpdateAsync(Guid userId, bool isAdmin, Guid inventoryId, UpdateInventoryRequest request)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);

        if (inventory == null)
            throw new NotFoundException("Inventory not found");

        if (!isAdmin && inventory.CreatedBy != userId)
            throw new ForbiddenException("You don't have access to the inventory");

        inventory.UpdateDetails(request.Title, request.Description, request.CategoryId, request.ImageUrl, request.IsPublic);

        var tags = await ResolveTagsAsync(request.Tags);

        inventory.SetTags(tags);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid userId, bool isAdmin, Guid inventoryId)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);

        if (inventory == null)
            throw new NotFoundException("Inventory not found");

        if (!isAdmin && inventory.CreatedBy != userId)
            throw new ForbiddenException("You don't have access to the inventory");

        _inventoryRepository.Delete(inventory);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Guid> AddCustomFieldAsync(
        Guid userId,
        bool isAdmin,
        Guid inventoryId,
        AddInventoryCustomFieldRequest request)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);

        if (inventory == null)
            throw new NotFoundException("Inventory not found");

        if (!isAdmin && inventory.CreatedBy != userId)
            throw new ForbiddenException("You don't have access to the inventory");

        var fieldId = inventory.AddCustomField(
            request.Title, 
            request.Description, 
            request.Type, 
            request.Order, 
            request.ShouldBeDisplayed);

        await _unitOfWork.SaveChangesAsync();

        return fieldId;
    }

    public async Task UpdateCustomFieldAsync(
        Guid userId,
        bool isAdmin,
        Guid inventoryId,
        Guid fieldId,
        UpdateInventoryCustomFieldRequest request)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId)
            ?? throw new NotFoundException("Inventory not found");

        if (!isAdmin && inventory.CreatedBy != userId)
            throw new ForbiddenException("You don't have access");

        inventory.UpdateCustomField(
            fieldId,
            request.Title,
            request.Description,
            request.Type,
            request.Order,
            request.ShouldBeDisplayed);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveCustomFieldsAsync(
        Guid userId,
        bool isAdmin,
        Guid inventoryId,
        List<Guid> fieldIds)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId)
            ?? throw new NotFoundException("Inventory not found");

        if (!isAdmin && inventory.CreatedBy != userId)
            throw new ForbiddenException("You don't have access");

        inventory.RemoveCustomFields(fieldIds);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<List<InventoryTag>> ResolveTagsAsync(IEnumerable<string> tagNames)
    {
        var normalized = tagNames
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim().ToLower())
            .Distinct()
            .ToList();

        if (!normalized.Any())
            return new List<InventoryTag>();

        var existingTags = await _inventoryTagRepository.GetByNamesAsync(normalized);

        var existingNames = existingTags
            .Select(t => t.Name)
            .ToHashSet();

        var newTags = normalized
            .Where(name => !existingNames.Contains(name))
            .Select(name => new InventoryTag(Guid.NewGuid(), name))
            .ToList();

        if (newTags.Any())
            await _inventoryTagRepository.AddRangeAsync(newTags);

        return existingTags.Concat(newTags).ToList();
    }
}
