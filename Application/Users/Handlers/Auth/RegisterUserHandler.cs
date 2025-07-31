using MediatR;
using Domain.Entities;
using Application.Dtos;
using Microsoft.Extensions.Logging;
using Domain.Interfaces.Repositories;
using Application.Users.Common.Exceptions;
using Application.Users.Common.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Application.Users.Common.Interface;

namespace Application.Users.Handlers;


public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, RegisterUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RegisterUserHandler> _logger;
    private readonly IDistributedCache _redis;
    private readonly IEmailService _emailService;

    public RegisterUserHandler(IUserRepository userRepository, ILogger<RegisterUserHandler> logger, IDistributedCache redis, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
        _redis = redis;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user is not null) throw new UserAlreadyExistException("User with this email already exist");

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = await GenerateUsername(request.FirstName, request.LastName),
                Email = request.Email,
                HashedPassword = hashPassword,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var isRegistered = await _userRepository.AddUserAsync(newUser);

            if (isRegistered)
            {

                var secret = Environment.GetEnvironmentVariable("EMAIL_SECRET") ?? throw new NullReferenceException("env variable for EMAIL_SECRET is not set");

                var token = EmailTokenGenerator.Generate(newUser.Email, secret, 15);

                await _redis.SetStringAsync($"verify:{newUser.Email}", token, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

                await _emailService.SendVerificationEmailAsync(newUser.FirstName, newUser.Email, token);

                _logger.LogInformation("==========✅ User registered and verification email sent to {Email}==========", newUser.Email);

                return new RegisterUserResponse
                {
                    UserId = newUser.UserId.ToString(),
                    Username = newUser.UserName,
                    Email = newUser.Email,
                    Token = token,
                    Message = "Registration successful. Please check your email to verify your account."
                };
            }

            return new RegisterUserResponse
            {
                Message = $"Registration of user {newUser.Email} failed"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "==========❌Error occured while registering user: " + request.Email + "==========");

            throw new Exception($"Error Onboarding user {request.Email}.");
        }
    }


    private async Task<string> GenerateUsername(string firstName, string lastName)
    {
        string lastPart = lastName.Length >= 3 
            ? lastName.Substring(0, 3).ToLower() 
            : lastName.ToLower();

        string baseUsername = $"{firstName.ToLower()}{lastPart}";

        string username = baseUsername;

        int suffix = 1;

        while (await _userRepository.UserExistByUsernameAsync(username))
        {
            username = $"{baseUsername}{suffix}";

            suffix++;
        }

        return username;
    }

}