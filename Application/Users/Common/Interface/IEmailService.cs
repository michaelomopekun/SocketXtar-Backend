namespace Application.Users.Common.Interface;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string token);
}

