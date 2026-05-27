using MediatR;
using UsersApi.DTOs;

namespace UsersApi.CQRS.Queries;

public record GetAllUserQuery : IRequest<List<UserResponseDTO>>;