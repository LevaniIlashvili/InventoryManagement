using InventoryManagement.Application.DTOs.Category;

namespace InventoryManagement.Application.Interfaces.Application;

public interface ICategoryService
{
    Task<List<CategoryDTO>> GetAllAsync();
}
