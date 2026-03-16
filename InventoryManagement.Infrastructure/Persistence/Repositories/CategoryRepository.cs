using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<InventoryCategory>> GetAllAsync()
    {
        return await _dbContext.InventoryCategories.ToListAsync();
    }
}
