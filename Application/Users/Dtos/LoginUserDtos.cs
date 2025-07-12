using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Application.Users.Dtos;

public class LoginUserRequest : IRequest<LoginUserResponse>
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
    [DataType(DataType.Password)]
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