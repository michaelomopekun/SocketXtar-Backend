using Microsoft.AspNetCore.Http;

namespace Application.Users.Common.Interface;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file);

}
