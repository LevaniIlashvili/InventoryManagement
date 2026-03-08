using InventoryManagement.Application.Interfaces.Application;
using InventoryManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IInventoryItemService, InventoryItemService>();

        return services;
    }
}
