namespace InventoryManagement.Domain.Entities;

public class InventoryAccess
{
    public Guid Id { get; set; }

    public Guid InventoryId { get; set; }
    public Inventory Inventory { get; set; }

    public Guid UserId { get; set; }
}
