using Application.Users.Common.Exceptions;
using Application.Users.Common.Interface;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;

        var account = new Account(
        Environment.GetEnvironmentVariable("CLOUDINARY_CLOUDNAME"),
        Environment.GetEnvironmentVariable("CLOUDINARY_APIKEY"),
        Environment.GetEnvironmentVariable("CLOUDINARY_APISECRET")
        ) ?? throw new EnvConfigurationException("Cloudinary configuration is missing");

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File cannot be null or empty", nameof(file));
        }

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "profile_pictures"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        return result.SecureUrl.ToString();
    }
}
