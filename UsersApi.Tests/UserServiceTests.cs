using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UsersApi.Data;
using UsersApi.Models;
using UsersApi.Services;
using Xunit;

namespace UsersApi.Tests;

public class UserServiceTests
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private IMemoryCache GetMemoryCache()
    {
        return new MemoryCache(new MemoryCacheOptions());
    }

    [Fact]
    public void GetAll_ReturnsAllUsers()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Users.AddRange(
            new User { Id = 1, Name = "Alice", Email = "alice@email.com", Age = 28, PasswordHash = "hash1" },
            new User { Id = 2, Name = "Bob", Email = "bob@email.com", Age = 34, PasswordHash = "hash2" }
        );
        context.SaveChanges();

        var service = new UserService(context, GetMemoryCache());

        // Act
        var result = service.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetById_ReturnsUser_WhenExists()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Users.Add(new User { Id = 1, Name = "Alice", Email = "alice@email.com", Age = 28, PasswordHash = "hash1" });
        context.SaveChanges();

        var service = new UserService(context, GetMemoryCache());

        // Act
        var result = service.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
    }

    [Fact]
    public void GetById_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var context = GetInMemoryContext();
        var service = new UserService(context, GetMemoryCache());

        // Act
        var result = service.GetById(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CreateReturnsCreatedUser()
    {
        //Arrange
        var context = GetInMemoryContext();
        var service = new UserService(context,GetMemoryCache());
        var newUser = new User
        {
            Name = "Dhruv",
            Email = "dhruv@test.com",
            Age = 22,
            PasswordHash = "hashedpassword"
        };

        //Act 
        var result = service.Create(newUser);

        //Assert
        Assert.NotNull(result);
        Assert.Equal("Dhruv",result.Name);
        Assert.Equal("dhruv@test.com",result.Email);
    }

    [Fact]
    public void Delete_ReturnsTrue_WhenUserExists()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Users.Add(new User
        {
            Id = 1,
            Name = "Alice",
            Email = "alice@gmail.com",
            Age = 28,
            PasswordHash = "hash1"
        });
        context.SaveChanges();
        var service = new UserService(context,GetMemoryCache());
    
        // Act
        var result = service.Delete(1);
    
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Delete_ReturnsFalse_WhenUserNotExists()
    {
        // Arrange
        var context = GetInMemoryContext();
        var service = new UserService(context,GetMemoryCache());
        // Act
        var result = service.Delete(99);
    
        // Assert
        Assert.False(result);
    }
}