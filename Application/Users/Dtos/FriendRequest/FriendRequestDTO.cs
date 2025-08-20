namespace Application.Users.Dtos.FriendRequest;

public class FriendRequestDTO
{
    public Guid Id { get; set; }
    public string SenderUserName { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
