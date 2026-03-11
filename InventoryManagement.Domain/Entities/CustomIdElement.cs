using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Domain.Entities;

public class CustomIdElement
{
    public Guid Id { get; set; }
    public Guid InventoryId { get; set; }
    public int Order { get; set; }
    public CustomIdElementType Type { get; set; }
    public string? FixedText { get; set; }
    public string? Format { get; set; }
}
