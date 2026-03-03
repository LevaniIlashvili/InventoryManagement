namespace InventoryManagement.Application.Interfaces.Infrastructure;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
