using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces.Infrastructure.Repositories;

public interface IInventoryTagRepository
{
    Task<List<InventoryTag>> GetAllAsync();
    Task AddRangeAsync(List<InventoryTag> tags);
    Task<List<InventoryTag>> GetByNamesAsync(IEnumerable<string> names);
}
