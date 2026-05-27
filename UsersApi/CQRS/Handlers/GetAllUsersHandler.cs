using MediatR;
using UsersApi.CQRS.Queries;
using UsersApi.DTOs;
using UsersApi.Services;

namespace UsersApi.CQRS.Handlers;

public class GetAllUsersHandler : IRequestHandler<GetAllUserQuery,List<UserResponseDTO>>
{
    private readonly IUserService _userservice;
    public GetAllUsersHandler(IUserService userservice)
    {
        _userservice = userservice;
    }
    public async Task<List<UserResponseDTO>>Handle(GetAllUserQuery request,CancellationToken cancellationToken)
    {
        // var users = _userservice.GetAll();
        // return Task.FromResult(users);
        return await _userservice.GetAllAsync();
    }
};