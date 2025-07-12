using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Application.Users.Dtos;

public class EmailVerificationRequest : IRequest<EmailVerificationResponse>
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class EmailVerificationResponse
{
    public string Message { get; set; } = string.Empty;
}