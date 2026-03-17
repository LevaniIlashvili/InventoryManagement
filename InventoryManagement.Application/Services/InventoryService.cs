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
    private readonly IInventoryReadRepository _inventoryReadRepository;
    private readonly IUserReadRepository _userReadRepository;

    public InventoryService(
        IInventoryRepository inventoryRepository,
        IInventoryTagRepository inventoryTagRepository,
        IUnitOfWork unitOfWork,
        IInventoryReadRepository inventoryReadRepository,
        IUserReadRepository userReadRepository)
    {
        _inventoryRepository = inventoryRepository;
        _inventoryTagRepository = inventoryTagRepository;
        _unitOfWork = unitOfWork;
        _inventoryReadRepository = inventoryReadRepository;
        _userReadRepository = userReadRepository;
    }

    public async Task RemoveUserFromAccessList(Guid removerId, bool isAdmin, Guid inventoryId, Guid userIdBeingRemoved)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);

        if (inventory == null)
        {
            throw new NotFoundException("Inventory not found");
        }

        if (!isAdmin && inventory.CreatedBy != removerId)
            throw new ForbiddenException("You don't have access to the inventory");

        var accessRecord = inventory.AccessList.FirstOrDefault(a => a.UserId == userIdBeingRemoved);

        if (accessRecord == null)
            throw new BadRequestException("User is not in the access list");

        inventory.AccessList.Remove(accessRecord);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddUserToAccessList(Guid adderId, bool isAdmin, Guid inventoryId, Guid userIdBeingAdded)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);

        if (inventory == null)
        {
            throw new NotFoundException("Inventory not found");
        }

        if (!isAdmin && inventory.CreatedBy != adderId)
            throw new ForbiddenException("You don't have access to the inventory");

        var userBeingAdded = await _userReadRepository.GetByIdAsync(userIdBeingAdded);

        if (userBeingAdded == null)
            throw new NotFoundException("User being added not found");

        if (inventory.AccessList.Any(a => a.UserId == userIdBeingAdded))
            throw new BadRequestException("User is already in access list");

        inventory.AccessList.Add(new InventoryAccess { 
                                    InventoryId = inventoryId, 
                                    UserId = userIdBeingAdded 
        });

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<InventoryDTO>> GetInventoriesByTagAsync(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return new List<InventoryDTO>();

        return await _inventoryReadRepository.GetInventoriesByTagAsync(tag);
    }

    public async Task<List<InventoryDTO>> SearchInventoriesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<InventoryDTO>();

        return await _inventoryReadRepository.SearchInventoriesAsync(searchTerm);
    }

    public async Task<GetInventoryStatisticsResponse> GetInventoryStatisticsAsync(Guid inventoryId)
    {
        return await _inventoryReadRepository.GetInventoryStatisticsAsync(inventoryId);
    }

    public async Task<List<InventoryDTO>> GetLatestInventoriesAsync()
    {
        return await _inventoryReadRepository.GetLatestInventoriesAsync();
    }

    public async Task<List<InventoryDTO>> GetPopularInventoriesAsync()
    {
        return await _inventoryReadRepository.GetPopularInventoriesAsync();
    }

    public async Task<List<InventoryDTO>> GetUserInventoriesAsync(Guid userId)
    {
        var inventories = await _inventoryReadRepository.GetUserInventoriesAsync(userId);

        return inventories;
    }

    public async Task<GetInventoryResponse> GetByIdAsync(Guid id)
    {
        var inventory = await _inventoryReadRepository.GetByIdAsync(id);

        if (inventory == null)
        {
            throw new NotFoundException("Inventory not found");
        }

        return inventory;
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

    public async Task DeleteAsync(Guid userId, bool isAdmin, List<Guid> ids)
    {
        var inventories = await _inventoryRepository.GetByIdsAsync(ids);

        if (inventories == null || inventories.Count == 0)
            throw new NotFoundException("Inventories not found");

        foreach (var inventory in inventories)
        {
            if (!isAdmin && inventory.CreatedBy != userId)
                throw new ForbiddenException("You don't have access to the inventory");

            _inventoryRepository.Delete(inventory);
        }

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

    public async Task<Guid> AddCustomIdElementAsync(
        Guid userId,
        bool isAdmin,
        Guid inventoryId,
        AddCustomIdElementRequest request)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId)
            ?? throw new NotFoundException("Inventory not found");

        if (!isAdmin && inventory.CreatedBy != userId)
            throw new ForbiddenException("You don't have access");

        var customIdElement = new CustomIdElement
        {
            Id = Guid.NewGuid(),
            InventoryId = inventory.Id,
            Order = request.Order,
            Type = request.Type,
            FixedText = request.FixedText,
            Format =request.Format
        };
        
        inventory.CustomIdElements.Add(customIdElement);

        await _unitOfWork.SaveChangesAsync();

        return customIdElement.Id;
    }

    public async Task UpdateCustomIdElementAsync(
        Guid userId,
        bool isAdmin,
        Guid inventoryId,
        Guid elementId,
        UpdateCustomIdElementRequest request)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId)
            ?? throw new NotFoundException("Inventory not found");

        if (!isAdmin && inventory.CreatedBy != userId)
            throw new ForbiddenException("You don't have access");

        var elementToUpdate = inventory.CustomIdElements.FirstOrDefault(e => e.Id == elementId);

        elementToUpdate.Order = request.Order;
        elementToUpdate.Type = request.Type;
        elementToUpdate.FixedText = request.FixedText;
        elementToUpdate.Format = request.Format;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveCustomIdElementsAsync(
           Guid userId,
           bool isAdmin,
           Guid inventoryId,
           List<Guid> elementIds)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId)
            ?? throw new NotFoundException("Inventory not found");

        if (!isAdmin && inventory.CreatedBy != userId)
            throw new ForbiddenException("You don't have access");

        var elementsToRemove = inventory.CustomIdElements
            .Where(e => elementIds.Contains(e.Id))
            .ToList();

        foreach (var element in elementsToRemove)
        {
            inventory.CustomIdElements.Remove(element);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
