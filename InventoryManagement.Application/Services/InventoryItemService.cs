using InventoryManagement.Application.DTOs.InventoryItem;
using InventoryManagement.Application.Exceptions;
using InventoryManagement.Application.Exceptionsl;
using InventoryManagement.Application.Interfaces.Application;
using InventoryManagement.Application.Interfaces.Infrastructure;
using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Services;

public class InventoryItemService : IInventoryItemService
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ICustomIdGenerator _customIdGenerator;

    public InventoryItemService(
        IInventoryItemRepository inventoryItemRepository,
        IUnitOfWork unitOfWork,
        IInventoryRepository inventoryRepository,
        ICustomIdGenerator customIdGenerator)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
        _inventoryRepository = inventoryRepository;
        _customIdGenerator = customIdGenerator;
    }

    public async Task<List<InventoryItemDTO>> GetItemsAsync(Guid inventoryId)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);

        if (inventory == null)
        {
            throw new NotFoundException("Inventory not found");
        }

        var inventoryItems = await _inventoryItemRepository.GetByInventoryId(inventoryId);

        return inventoryItems.Select(i => new InventoryItemDTO(
            i.Id,
            i.CustomId,
            i.InventoryId,
            i.CreatedBy,
            i.CreatedAt,
            i.Values.Select(i => new CustomFieldValueDTO(i.Id, i.InventoryCustomFieldId, i.Value)).ToList())).ToList();
    }

    public async Task<Guid> AddItemAsync(Guid userId, bool isAdmin, AddInventoryItemRequest request)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(request.InventoryId);

        if (inventory == null)
            throw new NotFoundException("Inventory not found");

        if (!inventory.IsPublic && !isAdmin && !inventory.AccessList.Any(a => a.UserId == userId) && inventory.CreatedBy != userId)
            throw new ForbiddenException("You don't have write access");

        var fieldDictionary = inventory.CustomFields.ToDictionary(f => f.Id, f => f);

        var invalidFieldIds = request.CustomFieldValues
            .Select(v => v.InventoryCustomFieldId)
            .Except(fieldDictionary.Keys)
            .ToList();

        if (invalidFieldIds.Any())
            throw new NotFoundException($"Custom fields not found: {string.Join(",", invalidFieldIds)}");

        var validationErrors = new List<string>();

        foreach (var cfv in request.CustomFieldValues)
        {
            if (string.IsNullOrWhiteSpace(cfv.Value))
                continue;

            var fieldDef = fieldDictionary[cfv.InventoryCustomFieldId];

            switch (fieldDef.Type)
            {
                case FieldType.Number:
                    if (!decimal.TryParse(cfv.Value, out _))
                        validationErrors.Add($"Field '{fieldDef.Title}' must be a valid number.");
                    break;

                case FieldType.Boolean:
                    if (!bool.TryParse(cfv.Value, out _))
                        validationErrors.Add($"Field '{fieldDef.Title}' must be true or false.");
                    break;

                case FieldType.SingleLineText:
                    if (cfv.Value.Contains('\n') || cfv.Value.Contains('\r'))
                        validationErrors.Add($"Field '{fieldDef.Title}' cannot contain multiple lines.");
                    break;

            }
        }

        if (validationErrors.Any())
        {
            throw new BadRequestException(string.Join(" ", validationErrors));
        }

        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            CustomId = await _customIdGenerator.GenerateId(inventory),
            InventoryId = request.InventoryId,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = userId,
            Values = request.CustomFieldValues.Select(cfv => new ItemFieldValue
            {
                Id = Guid.NewGuid(),
                InventoryCustomFieldId = cfv.InventoryCustomFieldId,
                Value = cfv.Value
            }).ToList()
        };

        var id = await _inventoryItemRepository.AddItemAsync(item);

        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    public async Task RemoveItemsAsync(Guid userId, bool isAdmin, List<Guid> itemIds)
    {
        foreach (var itemId in itemIds)
        {
            var item = await _inventoryItemRepository.GetByIdAsync(itemId);

            if (item == null)
                throw new NotFoundException($"Item {itemId} not found");

            var inventory = await _inventoryRepository.GetByIdAsync(item.InventoryId);

            if (inventory == null)
                throw new NotFoundException("Inventory not found");

            if (!inventory.IsPublic &&
                !isAdmin && 
                !inventory.AccessList.Any(a => a.UserId == userId) &&
                inventory.CreatedBy != userId)
            {
                throw new ForbiddenException("You don't have write access");
            }

            _inventoryItemRepository.RemoveItem(item);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(Guid userId, bool isAdmin, Guid itemId, List<AddCustomFieldValueDTO> customFields)
    {
        var item = await _inventoryItemRepository.GetByIdAsync(itemId);

        if (item == null)
            throw new NotFoundException("Item not found");

        var inventory = await _inventoryRepository.GetByIdAsync(item.InventoryId);

        if (inventory == null)
            throw new NotFoundException("Inventory not found");

        if (!inventory.IsPublic &&
            !isAdmin &&
            !inventory.AccessList.Any(a => a.UserId == userId) &&
            inventory.CreatedBy != userId)
            throw new ForbiddenException("You don't have write access");

        var inventoryCustomFieldIds = inventory.CustomFields
            .Select(f => f.Id)
            .ToHashSet();

        var invalidFieldIds = customFields
            .Select(v => v.InventoryCustomFieldId)
            .Except(inventoryCustomFieldIds)
            .ToList();

        if (invalidFieldIds.Any())
            throw new NotFoundException($"Custom fields not found: {string.Join(",", invalidFieldIds)}");

        foreach (var field in customFields)
        {
            var existingValue = item.Values
                .FirstOrDefault(v => v.InventoryCustomFieldId == field.InventoryCustomFieldId);

            if (existingValue != null)
            {
                existingValue.Value = field.Value;
            }
            else
            {
                item.Values.Add(new ItemFieldValue
                {
                    Id = Guid.NewGuid(),
                    InventoryCustomFieldId = field.InventoryCustomFieldId,
                    Value = field.Value
                });
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
