using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UsersApi.Data;
using UsersApi.DTOs;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
namespace UserApis.Tests;

public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "ThisIsASecretKeyForJwtToken12345!",
                    ["Jwt:Issuer"] = "UsersApi",
                    ["Jwt:Audience"] = "UsersApi"
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_Returns200_WithValidData()
    {
        // Arrange
        var request = new RegisterDTO
        {
            Name = "Dhruv",
            Email = "dhruv@test.com",
            Password = "Dhruv@123",
            Age = 22
        };

        // Act 
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);
        
        // Add this line to see the actual error
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task Register_Returns400_WithInvalidData()
    {
        // Arrange
        var request = new RegisterDTO
        {
            Name = "Dhruv",
            Email = "notanEmail",
            Password = "Dhruv123",
            Age = 22,
        };

        // Act 
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task Login_Return401_WithWrongPassword()
    {
        // Arrange
        var request = new RegisterDTO
        {
            Name = "Dhruv",
            Email = "dhruv2@test.com",
            Password = "Dhruv123",
            Age = 22
        };
        await _client.PostAsJsonAsync("/api/auth/register", request);

        var loginRequest = new LoginDTO
        {
            Email = "dhruv2@test.com",
            Password = "wrongPassword",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
};