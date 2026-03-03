namespace InventoryManagement.Domain.Entities;

public class InventoryCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public List<Inventory> Inventories { get; set; }
}
