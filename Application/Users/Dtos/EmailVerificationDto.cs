using MediatR;

namespace Application.Users.Dtos;

public class EmailVerificationRequest : IRequest<EmailVerificationResponse>
{
    public string Token = string.Empty;
    public string Email = string.Empty;
}

public class EmailVerificationResponse
{
    public string message = string.Empty;
}