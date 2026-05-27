
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersApi.Common;
using UsersApi.CQRS.Queries;
using UsersApi.DTOs;
using UsersApi.CQRS.Commands;

namespace UsersApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cqrs/users")]
[Authorize]

public class UsersCqrsController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersCqrsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _mediator.Send(new GetAllUserQuery());
        return Ok(ApiResponse<List<UserResponseDTO>>.Ok(users,"Users retreived successfully"));
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        if(user==null)
        {
            return NotFound(ApiResponse<UserResponseDTO>.Fail("User not Found"));
        }
        return Ok(ApiResponse<UserResponseDTO>.Ok(user,"Users retreived successfully"));
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>Delete(int id)
    {
        var result = await _mediator.Send(new DeleteUserCommand(id));
        if(!result)
        {
            return NotFound(ApiResponse<bool>.Fail("Users not found"));
        }
        return Ok(ApiResponse<bool>.Ok(true,"User deleted successfully"));
    }
}