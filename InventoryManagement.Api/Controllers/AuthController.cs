using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, ILogger<AuthController> logger, IConfiguration configuration)
    {
        _authService = authService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { message = "An error occurred during registration." });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Login failed");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = "An error occurred during login." });
        }
    }

    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth");
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };
        return Challenge(properties, "Google");
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync("Google");
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Google authentication failed." });
        }

        var claims = result.Principal?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
        var googleId = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var pictureUrl = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
        {
            return BadRequest(new { message = "Failed to retrieve user information from Google." });
        }

        var nameParts = name?.Split(' ') ?? Array.Empty<string>();
        var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
        var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

        var externalLoginRequest = new ExternalLoginRequest
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName,
            ProfilePictureUrl = pictureUrl,
            Provider = "Google"
        };

        try
        {
            var response = await _authService.HandleExternalLoginAsync(externalLoginRequest);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);

            var base64Response = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(jsonResponse));

            var frontendRedirectUrl = $"{_configuration["FrontendUrl"]}/oauth-callback?data={base64Response}";

            return Redirect(frontendRedirectUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google external login");
            return StatusCode(500, new { message = "An error occurred during external login." });
        }
    }

    [HttpGet("facebook")]
    public IActionResult FacebookLogin()
    {
        var redirectUrl = Url.Action(nameof(FacebookCallback), "Auth");
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };
        return Challenge(properties, "Facebook");
    }

    [HttpGet("facebook-callback")]
    public async Task<IActionResult> FacebookCallback()
    {
        var result = await HttpContext.AuthenticateAsync("Facebook");
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Facebook authentication failed." });
        }

        var claims = result.Principal?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
        var facebookId = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var pictureUrl = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

        if (string.IsNullOrEmpty(facebookId))
        {
            return BadRequest(new { message = "Failed to retrieve user information from Facebook." });
        }

        var nameParts = name?.Split(' ') ?? Array.Empty<string>();
        var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
        var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

        var userName = !string.IsNullOrEmpty(email) ? email : $"facebook_{facebookId}";
        var userEmail = !string.IsNullOrEmpty(email) ? email : $"{facebookId}@facebook.temp";

        var externalLoginRequest = new ExternalLoginRequest
        {
            Email = userEmail,
            UserName = userName,
            FirstName = firstName,
            LastName = lastName,
            ProfilePictureUrl = pictureUrl,
            Provider = "Facebook"
        };

        try
        {
            var response = await _authService.HandleExternalLoginAsync(externalLoginRequest);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
            var base64Response = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(jsonResponse));

            var frontendRedirectUrl = $"{_configuration["FrontendUrl"]}/oauth-callback?data={base64Response}";

            return Redirect(frontendRedirectUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Facebook external login");
            return StatusCode(500, new { message = "An error occurred during external login." });
        }
    }

    [HttpGet("test")]
    [Authorize]
    public IActionResult Test()
    {
        return Ok(new { message = "You are authenticated!", user = User.Identity?.Name });
    }
}