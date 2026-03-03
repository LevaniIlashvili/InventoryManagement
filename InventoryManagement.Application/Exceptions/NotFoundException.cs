namespace InventoryManagement.Application.Exceptionsl;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
