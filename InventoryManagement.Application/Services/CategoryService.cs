using InventoryManagement.Application.DTOs.Category;
using InventoryManagement.Application.Interfaces.Application;
using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;

namespace InventoryManagement.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryDTO>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return categories.Select(c => new CategoryDTO(c.Id, c.Name)).ToList();
    }
}
