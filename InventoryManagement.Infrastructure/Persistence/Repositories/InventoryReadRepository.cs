using InventoryManagement.Application.DTOs.Inventory;
using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

public class InventoryReadRepository : IInventoryReadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InventoryReadRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<InventoryDTO>> GetUserInventoriesAsync(Guid userId)
    {
        return await _dbContext.Inventories
                .AsNoTracking()
                .Where(i => i.CreatedBy == userId)
                .Select(i => new InventoryDTO(i.Id, i.Title, i.Description, i.CategoryId, i.IsPublic))
                .ToListAsync();
    }
}
