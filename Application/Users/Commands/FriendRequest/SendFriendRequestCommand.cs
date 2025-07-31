using Application.Users.Dtos.FriendRequest;
using MediatR;

namespace Application.Users.Commands.FriendRequest;

public record SendFriendRequestCommand(string SenderEmail, string ReceiverUserName) : IRequest<SendFriendRequestResponseDto>;
