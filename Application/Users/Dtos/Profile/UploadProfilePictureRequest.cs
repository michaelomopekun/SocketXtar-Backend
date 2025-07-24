using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Users.Dtos.Profile;

public class UploadProfilePictureRequest
{
    [Required]
    public IFormFile PictureFile { get; set; } = null!;
}
