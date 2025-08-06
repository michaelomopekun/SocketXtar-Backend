namespace Domain.Entities;

public class Friend
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FriendUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; } = null!;
    public User FriendUser { get; set; } = null!;
    
}
