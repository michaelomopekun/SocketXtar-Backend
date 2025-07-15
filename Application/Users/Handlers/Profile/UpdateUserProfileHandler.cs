using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers.Profile;

public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserProfileHandler> _logger;
    private readonly IDistributedCache _redis;


    public UpdateUserProfileHandler(IUserRepository userRepository, ILogger<UpdateUserProfileHandler> logger, IDistributedCache redis)
    {
        _userRepository = userRepository;
        _logger = logger;
        _redis = redis;
    }

    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating user profile for email: {Email}", request.Email);

            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(request.Request.UserName))
            {
                var userNameExists = await _userRepository.GetUserByUsernameAsync(request.Request.UserName);
                if (userNameExists is null && user.UserName != request.Request.UserName)
                {
                    user.UserName = request.Request.UserName;
                }
                else if (userNameExists != null && user.UserName == request.Request.UserName)
                {
                    _logger.LogWarning("Username {UserName} already exists", request.Request.UserName);

                    throw new Exception("Username already taken");
                }

            }

            if (!string.IsNullOrWhiteSpace(request.Request.Bio))
                user.Bio = request.Request.Bio;

            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userRepository.UpdateUserAsync(user);

            await _redis.RemoveAsync($"profile:{user.Email}");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error updating user profile for email: {Email}", request.Email);
            throw;
        }
    }

}
