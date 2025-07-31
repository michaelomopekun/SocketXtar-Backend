namespace Application.Users.Common.Interface;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string firstName, string toEmail, string token);
}

