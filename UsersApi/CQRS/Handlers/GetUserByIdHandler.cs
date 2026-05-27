using MediatR;
using UsersApi.CQRS.Queries;
using UsersApi.DTOs;
using UsersApi.Services;

namespace UsersApi.CQRS.Handlers;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserResponseDTO?>
{
    private readonly IUserService _userService;

    public GetUserByIdHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserResponseDTO?> Handle(GetUserByIdQuery request,CancellationToken cancellationToken)
    {
        // var user = _userService.GetById(request.Id);
        // return Task.FromResult(user);
        return await _userService.GetByIdAsync(request.Id);
    }
}