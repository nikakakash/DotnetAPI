using System.Net.Http.Json;
using System.Text.Json;

namespace DotnetAPI.Tests;

public class BaseTest
{
    protected readonly HttpClient _client;
    private const string BaseUrl = "http://localhost:5000"; // change to your port

    public BaseTest()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri(BaseUrl);
    }

    // Call this in any test that needs authentication
    protected async Task<string> GetAuthTokenAsync()
    {
        var loginBody = new
        {
            email = "nikatest@gmail.com", 
            password = "test"             
        };

        var response = await _client.PostAsJsonAsync("/Auth/Login", loginBody);
        var content = await response.Content.ReadAsStringAsync();

        // Extract token from response: {"token": "eyJhbG..."}
        var json = JsonDocument.Parse(content);
        return json.RootElement.GetProperty("token").GetString()!;
    }

    // Call this to make the HttpClient send the token automatically
    protected async Task AuthenticateAsync()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}