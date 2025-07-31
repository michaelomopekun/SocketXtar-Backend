namespace Application.Users.Dtos.FriendRequest;

public class SendFriendRequestResponseDto
{
    public string Message { get; init; } = string.Empty;
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; } = null;
}
