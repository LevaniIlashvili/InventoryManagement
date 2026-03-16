using InventoryManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Domain.Entities;

public class Inventory
{
    private const int MaxCustomFieldsPerType = 3;

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }

    public Guid CreatedBy { get; private set; }

    public Guid CategoryId { get; private set; }
    public InventoryCategory Category { get; private set; }

    public List<InventoryTag> Tags { get; private set; } = new();
    public string? ImageUrl { get; private set; }
    public bool IsPublic { get; private set; }

    public int CurrentSequence { get; set; } = 1;

    public DateTimeOffset CreatedAt { get; set; }

    public List<InventoryItem> Items { get; private set; } = new();

    private readonly List<InventoryCustomField> _customFields = new();
    public IReadOnlyCollection<InventoryCustomField> CustomFields => _customFields;

    public List<InventoryAccess> AccessList { get; set; } = new();

    public List<CustomIdElement> CustomIdElements { get; set; }

    private Inventory() { }

    public Inventory(
        Guid id,
        string title,
        string description,
        Guid createdBy,
        Guid categoryId,
        string? imageUrl,
        bool isPublic)
    {
        Id = id;
        Title = title;
        Description = description;
        CreatedBy = createdBy;
        CategoryId = categoryId;
        ImageUrl = imageUrl;
        IsPublic = isPublic;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateDetails(
        string title,
        string description,
        Guid categoryId,
        string? imageUrl,
        bool isPublic)
    {
        Title = title;
        Description = description;
        CategoryId = categoryId;
        ImageUrl = imageUrl;
        IsPublic = isPublic;
    }

    public Guid AddCustomField(
        string title,
        string description,
        FieldType type,
        int order,
        bool shouldBeDisplayed)
    {
        if (_customFields.Count(cf => cf.Type == type) >= MaxCustomFieldsPerType)
            throw new DomainException("Inventory cannot have more than 3 custom fields of same type");

        var customField = new InventoryCustomField(
            Guid.NewGuid(),
            Id,
            title,
            description,
            type,
            order,
            shouldBeDisplayed);

        _customFields.Add(customField);

        return customField.Id;
    }

    public void UpdateCustomField(
        Guid customFieldId,
        string title,
        string description,
        FieldType type,
        int order,
        bool shouldBeDisplayed)
    {
        var field = _customFields.FirstOrDefault(cf => cf.Id == customFieldId);

        if (field == null)
            throw new DomainException("Custom field not found");

        if (field.Type != type &&
            _customFields.Count(cf => cf.Type == type) >= MaxCustomFieldsPerType)
        {
            throw new DomainException("Inventory cannot have more than 3 custom fields of same type");
        }

        field.Update(title, description, type, order, shouldBeDisplayed);
    }

    public void RemoveCustomFields(IEnumerable<Guid> customFieldIds)
    {
        var ids = customFieldIds.ToHashSet();

        var fieldsToRemove = _customFields
            .Where(cf => ids.Contains(cf.Id))
            .ToList();

        if (!fieldsToRemove.Any())
            return;

        foreach (var field in fieldsToRemove)
            _customFields.Remove(field);
    }

    public void SetTags(IEnumerable<InventoryTag> tags)
    {
        Tags.Clear();

        foreach (var tag in tags.DistinctBy(t => t.Id))
        {
            Tags.Add(tag);
        }
    }
}