using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Interfaces.Infrastructure;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> HandleExternalLoginAsync(ExternalLoginRequest request);
}