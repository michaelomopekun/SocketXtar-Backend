using Application.Users.Common.Interface;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    public async Task SendVerificationEmailAsync(string toEmail, string token)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_KEY");

        var client = new SendGridClient(apiKey);

        var from = new EmailAddress("noreply@socketxtar.com", "SocketXtar");

        var subject = "Verify your SocketXtar email";

        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_BASE_URL"); 

        var link = $"{frontendUrl}/verify-email?token={token}&email={toEmail}";

        var htmlContent = $@"
            <div style='font-family: Arial, sans-serif; background-color: #f6f8fa; padding: 40px;'>
                <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.05); padding: 32px;'>
                    <h2 style='color: #2d3748; text-align: center;'>Welcome to SocketXtar!</h2>
                    <p style='color: #4a5568; font-size: 16px; text-align: center;'>
                        Thank you for registering. Please verify your email address to activate your account.
                    </p>
                    <div style='text-align: center; margin: 32px 0;'>
                        <a href='{link}' style='background: #4f46e5; color: #fff; padding: 14px 32px; border-radius: 6px; text-decoration: none; font-weight: bold; font-size: 16px; display: inline-block;'>
                            Verify Email
                        </a>
                    </div>
                    <p style='color: #718096; font-size: 14px; text-align: center;'>
                        If you did not create an account, you can safely ignore this email.
                    </p>
                    <hr style='margin: 32px 0; border: none; border-top: 1px solid #e2e8f0;'/>
                    <p style='color: #a0aec0; font-size: 12px; text-align: center;'>
                        &copy; {DateTime.UtcNow.Year} SocketXtar. All rights reserved.
                    </p>
                </div>
            </div>
        ";

        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);

        await client.SendEmailAsync(msg);
    }

}
