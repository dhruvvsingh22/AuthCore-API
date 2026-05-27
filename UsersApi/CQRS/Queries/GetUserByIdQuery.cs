using MediatR;
using UsersApi.DTOs;

namespace UsersApi.CQRS.Queries;

public record GetUserByIdQuery(int Id) : IRequest<UserResponseDTO?>;