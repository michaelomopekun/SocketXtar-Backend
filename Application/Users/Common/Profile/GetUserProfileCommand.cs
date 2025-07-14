using Application.Users.Dtos.Profile;
using MediatR;

namespace Application.Users.Common.Profile;

public record GetUserProfileCommand(string Email) : IRequest<ProfileDto>;
