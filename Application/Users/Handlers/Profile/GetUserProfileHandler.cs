using System.Text.Json;
using Application.Users.Common.Profile;
using Application.Users.Dtos.Profile;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers.Profile;


public class GetUserProfileHandler : IRequestHandler<GetUserProfileCommand, ProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserProfileHandler> _logger;
    private readonly IDistributedCache _redis;

    public GetUserProfileHandler(IUserRepository userRepository, ILogger<GetUserProfileHandler> logger, IDistributedCache redis)
    {
        _userRepository = userRepository;
        _logger = logger;
        _redis = redis;
    }

    public async Task<ProfileDto> Handle(GetUserProfileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching user profile for email: {Email}", request.Email);

        var cachekey = $"UserProfile:{request.Email}";
        var cachedProfile = await _redis.GetStringAsync(cachekey);

        if (cachedProfile != null)
        {
            _logger.LogInformation("User profile found in cache for email: {Email}", request.Email);
            return JsonSerializer.Deserialize<ProfileDto>(cachedProfile) ?? throw new NullReferenceException("Cached profile is null");
        }

        var user = await _userRepository.GetUserByEmailAsync(request.Email);

        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found", request.Email);
            throw new KeyNotFoundException($"User with email {request.Email} not found");
        }

        var profile = new ProfileDto
        {
            UserId = user.UserId.ToString(),
            UserName = user.UserName,
            Email = user.Email,
            Bio = user.Bio,
            ProfilePictureURL = user.ProfilePictureURL
        };

        var serializedProfile = JsonSerializer.Serialize(profile);
        
        await _redis.SetStringAsync(cachekey, serializedProfile, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
        });

        return profile;
    }
}
