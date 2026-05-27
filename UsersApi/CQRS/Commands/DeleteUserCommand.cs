using MediatR;

namespace UsersApi.CQRS.Commands;

public record DeleteUserCommand(int Id) : IRequest<bool>;