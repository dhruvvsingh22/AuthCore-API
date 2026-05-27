using Microsoft.AspNetCore.Mvc;
using UsersApi.Data;
using UsersApi.Models;
using UsersApi.Services;
using Microsoft.AspNetCore.Authorization;
using UsersApi.Common;
using UsersApi.DTOs;
using Asp.Versioning;
namespace UsersApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Authorize]
public class UsersController : ControllerBase
{
    // private readonly AppDbContext _context;
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    // public static List<User> _users = new()
    // {
    //     new User{Id = 1, Name ="Alice", Email = "alice@gmail.com",Age=28},
    //     new User{Id=2,Name="Bob",Email="bob@gmail.com",Age=34},
    //     new User{Id = 3,Name="Carol",Email="carol@gmail.com",Age=22},
    // };
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(ApiResponse<List<UserResponseDTO>>.Ok(users, "Users retrieved successfully"));
        // var users = _userService.GetAll();
        // return Ok(ApiResponse<List<User>>.Ok(users, "Users retrieved successfully"));
        // return Ok(_userService.GetAll());
        // var users = _context.Users.ToList();
        // return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse<UserResponseDTO>.Fail("User not found"));
        }
        return Ok(ApiResponse<UserResponseDTO>.Ok(user, "user retrieved successfully"));
        // var user = _userService.GetById(id);
        // if (user == null)
        // {
        //     return NotFound();
        // }
        // return Ok(user);
        // var user = _context.Users.FirstOrDefault(u => u.Id == id);
        // if (user == null)
        // {
        //     return NotFound();
        // }
        // return Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] User newUser)
    {
        var created = await _userService.CreateAsync(newUser);
        return CreatedAtAction(nameof(GetById), new { Id = created.Id },
        ApiResponse<UserResponseDTO>.Ok(created, "user created successfully"));
        // var created = _userService.Create(newUser);
        // return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        // _context.Users.Add(newUser);
        // _context.SaveChanges();
        // return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] User updatedUser)
    {
        var user = await _userService.UpdateAsync(id, updatedUser);
        if (user == null)
        {
            return NotFound(ApiResponse<UserResponseDTO>.Fail("User not found"));
        }
        return Ok(ApiResponse<UserResponseDTO>.Ok(user, "user created successfully"));
        // var user = _userService.Update(id, updatedUser);
        // if (user == null)
        // {
        //     return NotFound();
        // }
        // return Ok(user);
        // var user = _context.Users.FirstOrDefault(u => u.Id == id);
        // if (user == null) { return NotFound(); }

        // user.Name = updatedUser.Name;
        // user.Email = updatedUser.Email;
        // user.Age = updatedUser.Age;
        // _context.SaveChanges();
        // return Ok(user);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse<bool>.Fail("User not Found"));
        }
        return Ok(ApiResponse<bool>.Ok(true, "User deleted successfully"));
        // var result = _userService.Delete(id);
        // if (!result)
        // {
        //     return NotFound();
        // }
        // return NoContent();
        // var user = _context.Users.FirstOrDefault(u => u.Id == id);
        // if (user == null)
        // {
        //     return NotFound();
        // }
        // _context.Users.Remove(user);
        // _context.SaveChanges();
        // return NoContent();
    }

    // [HttpGet("error-test")]
    // public IActionResult TestError()
    // {
    //     throw new Exception("This is a great test error!");
    // }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;
        var result = await _userService.GetPagedAsync(page, pageSize);
        return Ok(ApiResponse<PageResponseDTO<UserResponseDTO>>.Ok(result, "users retreived successfully"));
    }
}