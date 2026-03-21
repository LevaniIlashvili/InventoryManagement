using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupportController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public SupportController(IConfiguration config)
    {
        _config = config;
        _httpClient = new HttpClient();
    }

    [HttpPost("ticket")]
    public async Task<IActionResult> CreateTicket([FromBody] TicketRequest request)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                     ?? User.Identity?.Name
                     ?? "UnknownUser";

        var adminEmails = _config["SupportSettings:AdminEmails"];

        var ticket = new
        {
            ReportedBy = userEmail,
            Inventory = request.InventoryTitle ?? "General",
            Link = request.Link,
            Priority = request.Priority,
            Summary = request.Summary,
            AdminEmails = adminEmails
        };

        string json = JsonSerializer.Serialize(ticket);

        var token = _config["Dropbox:AccessToken"];
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/upload");

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        string folderName = "InventorySupportTickets";
        string fileName = $"ticket_{DateTime.Now:yyyyMMddHHmmss}.json";

        string dropboxArg = $"{{\"path\": \"/{folderName}/{fileName}\",\"mode\": \"add\",\"autorename\": true,\"mute\": false}}";

        requestMessage.Headers.Add("Dropbox-API-Arg", dropboxArg);

        var fileBytes = Encoding.UTF8.GetBytes(json);
        requestMessage.Content = new ByteArrayContent(fileBytes);
        requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        var response = await _httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            return Ok(new { message = "Ticket submitted silently to Dropbox!" });
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            return StatusCode(500, $"Dropbox upload failed: {error}");
        }
    }
}

public record TicketRequest(string Priority, string Summary, string Link, string? InventoryTitle);