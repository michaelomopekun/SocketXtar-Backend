using MediatR;
using Domain.Interfaces.Repositories;
using Application.Users.Common.Interface;
using Application.Users.Common.Exceptions;
using Microsoft.Extensions.Caching.Distributed;

public class UploadProfilePictureHandler : IRequestHandler<UploadProfilePictureCommand, UploadProfilePictureResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IDistributedCache _redis;

    public UploadProfilePictureHandler(IUserRepository userRepository, ICloudinaryService cloudinaryService, IDistributedCache redis)
    {
        _userRepository = userRepository;
        _cloudinaryService = cloudinaryService;
        _redis = redis;
    }

    public async Task<UploadProfilePictureResponse> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null) throw new NotFoundException("User not found");

            var url = await _cloudinaryService.UploadImageAsync(request.File);

            user.ProfilePictureURL = url;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user);
            await _redis.RemoveAsync($"profile:{user.Email}");

            return new UploadProfilePictureResponse
            {
                Url = url
            };
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while uploading the profile picture", ex);
        }
    }
}
