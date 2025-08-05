namespace Application.Users.Dtos.FriendRequest;

public class FriendRequestDecisionResponseDTO
{
    public Guid RequestId { get; set; }
    public bool IsAccepted { get; set; }
    public DateTime DecisionTime { get; set; }
    public string Message { get; set; } = string.Empty;
    
}
