using InventoryManagement.Application.DTOs.Inventory;
using InventoryManagement.Application.Exceptionsl;
using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

public class InventoryReadRepository : IInventoryReadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InventoryReadRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<InventoryDTO>> GetInventoriesByTagAsync(string tag)
    {
        return await _dbContext.Inventories
            .AsNoTracking()
            .Where(i => i.Tags.Any(t => t.Name.ToLower() == tag.ToLower()))
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InventoryDTO(i.Id, i.Title, i.Description, i.CategoryId, i.IsPublic))
            .ToListAsync();
    }

    public async Task<List<InventoryDTO>> SearchInventoriesAsync(string searchTerm)
    { 
        return await _dbContext.Inventories
            .AsNoTracking()
            .Where(i => 
                EF.Functions.ToTsVector("english", i.Title)
                    .Matches(EF.Functions.WebSearchToTsQuery("english", searchTerm)) ||

                (i.Description != null && EF.Functions.ToTsVector("english", i.Description)
                    .Matches(EF.Functions.WebSearchToTsQuery("english", searchTerm)))
            )
            .Select(i => new InventoryDTO(i.Id, i.Title, i.Description, i.CategoryId, i.IsPublic))
            .ToListAsync();
    }

    public async Task<GetInventoryStatisticsResponse> GetInventoryStatisticsAsync(Guid inventoryId)
    {
        var inventory = await _dbContext.Inventories
            .Include(i => i.CustomFields)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == inventoryId);

        if (inventory == null)
            throw new NotFoundException("Inventory not found");

        var totalItems = await _dbContext.InventoryItems
            .CountAsync(i => i.InventoryId == inventoryId);

        if (totalItems == 0)
            return new GetInventoryStatisticsResponse(0, new List<NumericFieldStatistic>(), new List<StringFieldStatistic>());

        var numericFields = inventory.CustomFields.Where(f => f.Type == FieldType.Number).ToList();
        var stringFields = inventory.CustomFields.Where(f => f.Type == FieldType.SingleLineText).ToList();

        var numericStats = new List<NumericFieldStatistic>();
        var stringStats = new List<StringFieldStatistic>();

        var inventoryValues = _dbContext.InventoryItems
            .Where(i => i.InventoryId == inventoryId)
            .SelectMany(i => i.Values);

        foreach (var field in numericFields)
        {
            var dbStats = await inventoryValues
                .Where(v => v.InventoryCustomFieldId == field.Id && !string.IsNullOrEmpty(v.Value))
                .Select(v => Convert.ToDecimal(v.Value))
                .GroupBy(v => 1)
                .Select(g => new
                {
                    Min = g.Min(),
                    Max = g.Max(),
                    Avg = g.Average()
                })
                .FirstOrDefaultAsync();

            if (dbStats != null)
            {
                numericStats.Add(new NumericFieldStatistic(
                    field.Id,
                    field.Title,
                    dbStats.Min,
                    dbStats.Max,
                    Math.Round(dbStats.Avg, 2)
                ));
            }
        }

        foreach (var field in stringFields)
        {
            var topValues = await inventoryValues
                .Where(v => v.InventoryCustomFieldId == field.Id && !string.IsNullOrEmpty(v.Value))
                .GroupBy(v => v.Value.Trim())
                .Select(g => new { Value = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToDictionaryAsync(g => g.Value, g => g.Count);

            if (topValues.Any())
            {
                stringStats.Add(new StringFieldStatistic(
                    field.Id,
                    field.Title,
                    topValues
                ));
            }
        }

        return new GetInventoryStatisticsResponse(totalItems, numericStats, stringStats);
    }

    public async Task<List<InventoryDTO>> GetLatestInventoriesAsync()
    {
        return await _dbContext.Inventories
                .AsNoTracking()
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .Select(i => new InventoryDTO(i.Id, i.Title, i.Description, i.CategoryId, i.IsPublic))
                .ToListAsync();
    }

    public async Task<List<InventoryDTO>> GetPopularInventoriesAsync()
    {
        return await _dbContext.Inventories
                .AsNoTracking()
                .OrderByDescending(i => i.Items.Count)
                .Take(5)
                .Select(i => new InventoryDTO(i.Id, i.Title, i.Description, i.CategoryId, i.IsPublic))
                .ToListAsync();
    }

    public async Task<List<InventoryDTO>> GetUserInventoriesAsync(Guid userId)
    {
        return await _dbContext.Inventories
                .AsNoTracking()
                .Where(i => i.CreatedBy == userId)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new InventoryDTO(i.Id, i.Title, i.Description, i.CategoryId, i.IsPublic))
                .ToListAsync();
    }
}
