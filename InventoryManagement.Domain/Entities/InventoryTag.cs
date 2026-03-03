using InventoryManagement.Domain.Exceptions;

namespace InventoryManagement.Domain.Entities;

public class InventoryTag
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public List<Inventory> Inventories { get; private set; }

    private InventoryTag() { }

    public InventoryTag(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tag name cannot be empty");

        Id = id;
        Name = name.Trim().ToLower();
    }
}
