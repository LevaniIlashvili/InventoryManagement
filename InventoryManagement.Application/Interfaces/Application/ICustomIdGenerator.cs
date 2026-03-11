using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces.Application;

public interface ICustomIdGenerator
{
    Task<string> GenerateId(Inventory inventory);
}
