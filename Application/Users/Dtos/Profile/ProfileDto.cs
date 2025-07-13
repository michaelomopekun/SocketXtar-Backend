namespace Application.Users.Dtos.Profile;

public class ProfileDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ProfilePictureURL { get; set; }
}
