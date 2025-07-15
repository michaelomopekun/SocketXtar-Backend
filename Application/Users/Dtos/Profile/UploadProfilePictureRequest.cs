using Microsoft.AspNetCore.Http;

namespace Application.Users.Dtos.Profile;

public class UploadProfilePictureRequest
{
    public IFormFile PictureFile { get; set; } = null!;
}
