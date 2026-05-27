using MediatR;
using UsersApi.CQRS.Commands;
using UsersApi.Services;

namespace UsersApi.CQRS.Handlers;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserService _userService;

    public DeleteUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<bool> Handle(DeleteUserCommand request,CancellationToken cancellationToken)
    {
        // var result = _userService.Delete(request.Id);
        // return Task.FromResult(result);
        return await _userService.DeleteAsync(request.Id);
    }
}