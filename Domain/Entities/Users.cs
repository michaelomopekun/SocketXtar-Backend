using Domain.Enums;

namespace Domain.Entities;

public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string HashedPassword  { get; set; } = string.Empty;
    public Status UserStatus { get; set; } = Status.offline;
    public UserRoles UserRole { get; set; } = UserRoles.User;
    public bool EmailConfirmed { get; set; } = false;
    public string? ProfilePictureURL { get; set; } = string.Empty;
    public string? Bio { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LastLoggedInAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
