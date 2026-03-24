using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CrmController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public CrmController(IConfiguration config)
    {
        _config = config;
        _httpClient = new HttpClient();
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncToSalesforce([FromBody] CrmRequest request)
    {
        try
        {
            var sfConfig = _config.GetSection("Salesforce");

            string fullPassword = sfConfig["Password"] + sfConfig["SecurityToken"];

            var authContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", sfConfig["ClientId"]!),
                new KeyValuePair<string, string>("client_secret", sfConfig["ClientSecret"]!),
                new KeyValuePair<string, string>("username", sfConfig["Username"]!),
                new KeyValuePair<string, string>("password", fullPassword)
            });

            var authResponse = await _httpClient.PostAsync("https://login.salesforce.com/services/oauth2/token", authContent);

            if (!authResponse.IsSuccessStatusCode)
            {
                var authError = await authResponse.Content.ReadAsStringAsync();
                return StatusCode(500, $"Salesforce Auth Failed: {authError}");
            }

            var authData = JsonDocument.Parse(await authResponse.Content.ReadAsStringAsync());
            string accessToken = authData.RootElement.GetProperty("access_token").GetString()!;
            string instanceUrl = authData.RootElement.GetProperty("instance_url").GetString()!;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string apiVersion = "v58.0";

            var accountData = new { Name = request.CompanyName ?? $"{request.LastName} Household" };
            var accountJson = new StringContent(JsonSerializer.Serialize(accountData), Encoding.UTF8, "application/json");

            var accountResponse = await _httpClient.PostAsync($"{instanceUrl}/services/data/{apiVersion}/sobjects/Account", accountJson);

            if (!accountResponse.IsSuccessStatusCode) return StatusCode(500, "Failed to create Account in Salesforce.");

            var accountResult = JsonDocument.Parse(await accountResponse.Content.ReadAsStringAsync());
            string accountId = accountResult.RootElement.GetProperty("id").GetString()!;

            var contactData = new
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                AccountId = accountId
            };

            var contactJson = new StringContent(JsonSerializer.Serialize(contactData), Encoding.UTF8, "application/json");
            var contactResponse = await _httpClient.PostAsync($"{instanceUrl}/services/data/{apiVersion}/sobjects/Contact", contactJson);

            if (!contactResponse.IsSuccessStatusCode) return StatusCode(500, "Failed to create Contact in Salesforce.");

            return Ok(new { message = "Successfully synced user to Salesforce CRM!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal CRM Error: {ex.Message}");
        }
    }
}

public record CrmRequest(string FirstName, string LastName, string Email, string? CompanyName, string? Phone);
