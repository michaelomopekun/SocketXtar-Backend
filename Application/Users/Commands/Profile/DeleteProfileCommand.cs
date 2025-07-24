using Application.Users.Dtos.Profile;
using MediatR;

namespace Application.Users.Commands.Profile;

public record DeleteProfileCommand(string Email) : IRequest<DeleteProfileResponseDTO>;