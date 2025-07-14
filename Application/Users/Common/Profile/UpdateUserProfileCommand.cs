using Application.Dtos;
using MediatR;

public record UpdateUserProfileCommand(string Email, UpdateProfileRequest Request) : IRequest<bool>;
