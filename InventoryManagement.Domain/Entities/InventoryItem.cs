namespace InventoryManagement.Domain.Entities;

public class InventoryItem
{
    public Guid Id { get; set; }

    public Guid InventoryId { get; set; }
    public Inventory Inventory { get; set; }

    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CustomId { get; set; }
}
