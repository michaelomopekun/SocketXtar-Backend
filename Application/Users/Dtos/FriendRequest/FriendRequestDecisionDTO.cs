namespace Application.Users.Dtos.FriendRequest;

public class FriendRequestDecisionDTO
{
    public Guid RequestId { get; set; }
    public bool IsAccepted { get; set; } = false;
}
