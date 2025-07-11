using Application.Users.Common.Interface;
using Application.Users.Dtos;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers;

public class LoginUserHandler : IRequestHandler<LoginUserRequest, LoginUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoginUserHandler> _logger;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(IUserRepository userRepository, ILogger<LoginUserHandler> logger, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }
    
    
    public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword)) return new LoginUserResponse { Message = "Invalid email or password" };

        if (!user.EmailConfirmed) return new LoginUserResponse { Message = "Email not confirmed. Please check your email for the confirmation link." };

        var token = _tokenService.GenerateToken(user);

        _logger.LogInformation("✅ User logged in: {Email}", user.Email);

        return new LoginUserResponse
        {
            UserId = user.UserId.ToString(),
            Token = token,
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            Message = "Login successful",
            ProfilePictureUrl = user.ProfilePictureURL
        };
    }
}
