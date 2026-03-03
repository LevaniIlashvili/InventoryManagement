using InventoryManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        string[] roles = { "User", "Admin" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (!await context.InventoryCategories.AnyAsync())
        {
            var testCategory = new InventoryCategory()
            {
                Id = Guid.NewGuid(),
                Name = "Electronics"
            };

            context.InventoryCategories.Add(testCategory);
        }

        await context.SaveChangesAsync();
    }
}