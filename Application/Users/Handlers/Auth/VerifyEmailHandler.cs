using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.Users.Common.Exceptions;
using Application.Users.Dtos;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Application.Users.Handlers.Auth;

public class VerifyEmailHandler : IRequestHandler<EmailVerificationRequest, EmailVerificationResponse>
{
    private readonly IDistributedCache _redis;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VerifyEmailHandler> _logger;

    public VerifyEmailHandler(IDistributedCache redis, IUserRepository userRepository, ILogger<VerifyEmailHandler> logger)
    {
        _redis = redis;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<EmailVerificationResponse> Handle(EmailVerificationRequest request, CancellationToken cancellationToken)
    {

        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token)) throw new NullReferenceException("email and token must be provided");

            var storedToken = await _redis.GetStringAsync($"verify:{request.Email}");

            if (storedToken is null || storedToken != request.Token) throw new InvalidOrExpiredTokenException("Invalid or expired token.");

            var handler = new JwtSecurityTokenHandler();

            var secret = Environment.GetEnvironmentVariable("EMAIL_SECRET") ?? throw new NullReferenceException("env var EMAIL_SECRET cant be found");

            var key = Encoding.ASCII.GetBytes(secret);

            handler.ValidateToken(request.Token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true
            }, out SecurityToken _);

            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null) throw new UnauthorizedAccessException("user does not exist");

            user.EmailConfirmed = true;

            var update = await _userRepository.UpdateUserAsync(user);

            if (update == true)
            {
                await _redis.RemoveAsync($"verify:{request.Email}");

                _logger.LogInformation("==========✅ Email verified successfully.==========");

                return new EmailVerificationResponse { message = "Email verified successfully." };
            }

            return new EmailVerificationResponse { message = "Email verification Failed." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "==========❌Error while processing email verification==========");

            throw new Exception("Error while processing email verification");
        }



    }
}