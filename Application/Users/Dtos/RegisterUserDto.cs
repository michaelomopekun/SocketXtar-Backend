using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Application.Dtos;

public class RegisterUserRequest : IRequest<RegisterUserResponse>
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class RegisterUserResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
