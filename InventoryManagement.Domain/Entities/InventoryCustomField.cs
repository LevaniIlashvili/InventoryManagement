using InventoryManagement.Domain.Exceptions;

namespace InventoryManagement.Domain.Entities;

public class InventoryCustomField
{
    public Guid Id { get; private set; }

    public Guid InventoryId { get; private set; }
    public Inventory Inventory { get; private set; } = null!;

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public bool ShouldBeDisplayed { get; private set; }

    public FieldType Type { get; private set; }

    public int Order { get; private set; }

    public List<ItemFieldValue> ItemValues { get; set; } = new();

    private InventoryCustomField() { }

    internal InventoryCustomField(
        Guid id,
        Guid inventoryId,
        string title,
        string description,
        FieldType type,
        int order,
        bool shouldBeDisplayed)
    {
        Id = id;
        InventoryId = inventoryId;

        SetTitle(title);
        SetDescription(description);

        Type = type;
        Order = order;
        ShouldBeDisplayed = shouldBeDisplayed;
    }

    public void Update(
        string title,
        string description,
        FieldType type,
        int order,
        bool shouldBeDisplayed)
    {
        SetTitle(title);
        SetDescription(description);

        Type = type;
        Order = order;
        ShouldBeDisplayed = shouldBeDisplayed;
    }

    public void SetDisplay(bool shouldBeDisplayed)
    {
        ShouldBeDisplayed = shouldBeDisplayed;
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Custom field title cannot be empty");

        if (title.Length > 200)
            throw new DomainException("Custom field title cannot exceed 200 characters");

        Title = title;
    }

    private void SetDescription(string description)
    {
        if (description.Length > 1000)
            throw new DomainException("Custom field description cannot exceed 1000 characters");

        Description = description;
    }
}
