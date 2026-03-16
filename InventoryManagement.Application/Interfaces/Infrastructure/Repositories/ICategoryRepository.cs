using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces.Infrastructure.Repositories;

public interface ICategoryRepository
{
    Task<List<InventoryCategory>> GetAllAsync();
}
