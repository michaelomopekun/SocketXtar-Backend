namespace Domain.Entities;

public class FriendRequest
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderUserName { get; set; } = string.Empty;
    public string ReceiverUserName { get; set; } = string.Empty;
    public Guid ReceiverId { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsAccepted { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public User Sender { get; set; } = new User();
    public User Receiver { get; set; } = new User();
    
}
