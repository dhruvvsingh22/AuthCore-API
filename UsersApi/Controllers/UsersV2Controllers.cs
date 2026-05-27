using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersApi.Common;
using UsersApi.DTOs;
using UsersApi.Services;

namespace UsersApi.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/users")]
[Authorize]
public class UsersV2Controller : ControllerBase
{
    private readonly IUserService _userService;

    public UsersV2Controller(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        
        // V2 returns extra metadata
        return Ok(ApiResponse<object>.Ok(new
        {
            users,
            totalCount = users.Count,
            version = "2.0",
            timestamp = DateTime.UtcNow
        }, "Users retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<UserResponseDTO>.Fail("User not found"));
        return Ok(ApiResponse<UserResponseDTO>.Ok(user, "User retrieved successfully"));
    }
}