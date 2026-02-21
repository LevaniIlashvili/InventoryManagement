using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        existingUser = await _userManager.FindByNameAsync(request.UserName);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this username already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        if (await _roleManager.RoleExistsAsync("User"))
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwtTokenService.GenerateToken(user, roles);

        return new AuthResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        if (user.IsBlocked)
        {
            throw new UnauthorizedAccessException("Your account has been blocked.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwtTokenService.GenerateToken(user, roles);

        return new AuthResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            }
        };
    }

    public async Task<AuthResponse> HandleExternalLoginAsync(ExternalLoginRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser == null)
        {
            var userName = !string.IsNullOrEmpty(request.UserName)
                ? request.UserName
                : request.Email;

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                ProfilePictureUrl = request.ProfilePictureUrl,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            existingUser = user;
        }
        else
        {
            if (!string.IsNullOrEmpty(request.FirstName))
                existingUser.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName))
                existingUser.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.ProfilePictureUrl))
                existingUser.ProfilePictureUrl = request.ProfilePictureUrl;

            await _userManager.UpdateAsync(existingUser);
        }

        if (existingUser.IsBlocked)
        {
            throw new UnauthorizedAccessException("Your account has been blocked.");
        }

        var roles = await _userManager.GetRolesAsync(existingUser);
        if (!roles.Any() && await _roleManager.RoleExistsAsync("User"))
        {
            await _userManager.AddToRoleAsync(existingUser, "User");
            roles = await _userManager.GetRolesAsync(existingUser);
        }

        var token = _jwtTokenService.GenerateToken(existingUser, roles);

        return new AuthResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = existingUser.Id,
                UserName = existingUser.UserName ?? string.Empty,
                Email = existingUser.Email ?? string.Empty,
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                Roles = roles.ToList()
            }
        };
    }
}
