namespace InventoryManagement.Domain.Entities;

public class ItemFieldValue
{
    public Guid Id { get; set; }

    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; }
    
    public Guid InventoryCustomFieldId { get; set; }
    public InventoryCustomField CustomField { get; set; }


}
