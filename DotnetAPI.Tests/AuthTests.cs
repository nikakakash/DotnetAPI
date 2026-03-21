using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace DotnetAPI.Tests;

public class AuthTests
{
    private readonly HttpClient _client;

    public AuthTests()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5000");
    }

    [Fact]
    public async Task Register_WithNewUser_Returns200()
    {
        var newUser = new
        {
            firstName = "Test",
            lastName = "User",
            email = $"user_{Guid.NewGuid()}@test.com",
            password = "TestPassword123",
            passwordConfirm = "TestPassword123"
        };

        var response = await _client.PostAsJsonAsync("/Auth/Register", newUser);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ThrowsException()
    {
        var user = new
        {
            firstName = "Test",
            lastName = "User",
            email = "nikatest@gmail.com",
            password = "TestPassword123",
            passwordConfirm = "TestPassword123"
        };

        var response = await _client.PostAsJsonAsync("/Auth/Register", user);

        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_WithMismatchedPasswords_Returns400()
    {
        var user = new
        {
            firstName = "Test",
            lastName = "User",
            email = $"mismatch_{Guid.NewGuid()}@test.com",
            password = "Password1",
            passwordConfirm = "Password2"
        };

        var response = await _client.PostAsJsonAsync("/Auth/Register", user);

        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Login_WithValidCredentials_Returns200()
    {
        var loginBody = new
        {
            email = "nikatest@gmail.com",
            password = "test"
        };

        var response = await _client.PostAsJsonAsync("/Auth/Login", loginBody);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var loginBody = new
        {
            email = "nikatest@gmail.com",
            password = "WRONG_PASSWORD"
        };

        var response = await _client.PostAsJsonAsync("/Auth/Login", loginBody);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithEmptyFields_ReturnsNot200()
    {
        var loginBody = new
        {
            email = "",
            password = ""
        };

        var response = await _client.PostAsJsonAsync("/Auth/Login", loginBody);

        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }
}
