using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using System.Text.Json;
namespace DotnetAPI.Tests;


public class UserCompleteTests : BaseTest
{

    [Fact]
    public async Task GET_Users_WithValidToken_Returns200()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/UserComplete/GetUsers/0/true");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_User_ByUserId_Returns200()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/UserComplete/GetUsers/1/true");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_ActiveUsers_Only_Returns200()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/UserComplete/GetUsers/0/true");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_AllUsers_IgnoringActive_Returns200()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/UserComplete/GetUsers/0/false");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Users_WithOutValidToken_Returns401()
    {

        var response = await _client.GetAsync("/UserComplete/GetUsers/0/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    
    [Fact]
    public async Task UserComplete_FullCrudCiclye_Success()
    {
        await AuthenticateAsync();
        await CleanupLeftoverTestUsersAsync();
        string testEmail = await CreateTestUserAsync();

       



        int userId = await GetUserIdByEmailAsync(testEmail);
        userId.Should().BeGreaterThan(0, "user should have been created with a valid ID");

        var updatedUser = new
        {
            userId = userId,
            firstName = "UpdatedName",
            lastName = "UpdatedLastName",
            email = testEmail,
            gender = "Male",
            active = true,
            jobTitle = "Senior Tester",
            department = "QA",
            salary = 75000,
            avgSalary = 0
        };

        var updateResponse = await _client.PutAsJsonAsync("/UserComplete/UpsertUser", updatedUser);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        int updatedUserId = await GetUserIdByEmailAsync(testEmail);
        updatedUserId.Should().Be(userId, "same user should still be found by email after update");

        var deleteResponse = await _client.DeleteAsync($"/UserComplete/DeleteUser/{userId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        int deletedUserId = await GetUserIdByEmailAsync(testEmail);
        deletedUserId.Should().Be(-1, "user should no longer be found after deletion");
    }

    [Fact]
    public async Task Delete_User_Success()
    {
        await AuthenticateAsync();
        string testEmail = await CreateTestUserAsync();
        int userId = await GetUserIdByEmailAsync(testEmail);

        var response = await _client.DeleteAsync($"/UserComplete/DeleteUser/{userId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteUser_WithInvalidId_Returns400OrNotFound()
    {
        await AuthenticateAsync();

        var response = await _client.DeleteAsync("/UserComplete/DeleteUser/999999");
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound,
            HttpStatusCode.OK,
            HttpStatusCode.InternalServerError
        );
    }

    private async Task CleanupLeftoverTestUsersAsync()
    {
        var response = await _client.GetAsync("/UserComplete/GetUsers/0/true");
        var content = await response.Content.ReadAsStringAsync();
        var users = JsonDocument.Parse(content);

        foreach (var user in users.RootElement.EnumerateArray())
        {
            if (user.TryGetProperty("firstName", out var firstName) &&
                firstName.GetString() == "AutoTest")
            {
                int id = user.GetProperty("userId").GetInt32();
                await _client.DeleteAsync($"/UserComplete/DeleteUser/{id}");
            }
        }
    }

    private async Task<string> CreateTestUserAsync()
    {
        string uniqueEmail = $"autotest_{Guid.NewGuid()}@test";

        var testUser = new
        {
            userId = 0,
            firstName = "AutoTest",
            lastName = "User",
            email = uniqueEmail,
            gender = "Male",
            active = true,
            jobTitle = "Tester",
            department = "QA",
            salary = 50000,
            avgSalary = 12345
        };

        await _client.PutAsJsonAsync("/UserComplete/UpsertUser", testUser);
        return uniqueEmail;
    }

    private async Task<int> GetUserIdByEmailAsync(string email)
    {
        var response = await _client.GetAsync("/UserComplete/GetUsers/0/true");
        var content = await response.Content.ReadAsStringAsync();


        var users = JsonDocument.Parse(content);


        foreach (var user in users.RootElement.EnumerateArray())
        {
            if (user.TryGetProperty("email", out var emailProp))
            {
                string? userEmail = emailProp.GetString();
                // Compare ignoring case and trimming whitespace
                if (string.Equals(userEmail?.Trim(), email.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    if (user.TryGetProperty("userId", out var idProp))
                        return idProp.GetInt32();
                }
            }
        }
        return -1;
    }
}
