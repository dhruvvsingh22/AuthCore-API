using Microsoft.AspNetCore.Mvc;
using Moq;
using UsersApi.Common;
using UsersApi.Controllers;
using UsersApi.DTOs;
using UsersApi.Models;
using UsersApi.Services;
using Xunit;
namespace UserApi.Tests;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockService;
    private readonly UsersController _controller;
    public UserControllerTests()
    {
        _mockService = new Mock<IUserService>();
        _controller = new UsersController(_mockService.Object);
    }
    [Fact]
    public void GetAll_Returns200_WithUsers()
    {
        var fakeUsers = new List<UserResponseDTO>
        {
            new UserResponseDTO{Id = 1,Name = "Alice",Email = "Alice@gmail.com",Age = 28 },
            new UserResponseDTO{Id = 2,Name = "Bob",Email = "Bob@gmail.com",Age = 28 },
        };

        _mockService.Setup(s => s.GetAll()).Returns(fakeUsers);

        //Act
        var actionResult = _controller.GetAll();
        var result = actionResult as OkObjectResult;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(200,result.StatusCode);
    }
    [Fact]
    public void GetById_Returns404_WhenUserNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetById(99)).Returns((UserResponseDTO?)null);

        // Act
        var actionResult = _controller.GetById(99);
        var result = actionResult as NotFoundObjectResult;
        // Asssert
        Assert.NotNull(result);
        Assert.Equal(404,result.StatusCode);
    }

    [Fact]
    public void GetById_Returns200_WhenUserFound()
    {
        // Arrange
        var fakeUser = new UserResponseDTO 
        { 
            Id = 1, 
            Name = "Alice", 
            Email = "alice@email.com", 
            Age = 28 
        };

        _mockService.Setup(s => s.GetById(1)).Returns(fakeUser);

        // Act
        var actionResult = _controller.GetById(1);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
}