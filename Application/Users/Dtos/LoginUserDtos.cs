using MediatR;

namespace Application.Users.Dtos;

public class LoginUserRequest : IRequest<LoginUserResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}


public class LoginUserResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; } = null;
}