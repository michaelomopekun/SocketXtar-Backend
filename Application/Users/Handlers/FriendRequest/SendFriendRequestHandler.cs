using Application.Users.Commands.FriendRequest;
using Application.Users.Dtos.FriendRequest;
using Domain.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers.FriendRequest;

public class SendFriendRequestHandler : IRequestHandler<SendFriendRequestCommand, SendFriendRequestResponseDto>
{
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly IUserRepository _userRepository;
    public readonly ILogger<SendFriendRequestHandler> _logger;
    public readonly IDistributedCache _redis;

    public SendFriendRequestHandler(IFriendRequestRepository friendRequestRepository, IUserRepository userRepository, ILogger<SendFriendRequestHandler> logger, IDistributedCache redis)
    {
        _friendRequestRepository = friendRequestRepository;
        _userRepository = userRepository;
        _logger = logger;
        _redis = redis;
    }

    public async Task<SendFriendRequestResponseDto> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.SenderEmail) || string.IsNullOrWhiteSpace(request.ReceiverUserName))
            {
                return new SendFriendRequestResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Sender email and Receiver username cannot be empty."
                };
            }

            var sender = await _userRepository.GetUserByEmailAsync(request.SenderEmail);
            if (sender == null)
            {
                return new SendFriendRequestResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Sender not found."
                };
            }

            var requestExists = await _friendRequestRepository.GetFriendRequestBySenderAndReceiverUsernameAsync(sender.UserName, request.ReceiverUserName);

            if (requestExists != null)
            {
                return new SendFriendRequestResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Friend request already sent."
                };
            }

            var receiver = await _userRepository.GetUserByUsernameAsync(request.ReceiverUserName);
            if (sender == null || receiver == null)
            {
                return new SendFriendRequestResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Sender or Receiver not found."
                };
            }

            if (sender.UserId == receiver.UserId)
            {
                return new SendFriendRequestResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = "You cannot send a friend request to yourself."
                };
            }

            var friendRequest = new Domain.Entities.FriendRequest
            {
                Id = Guid.NewGuid(),
                SenderId = sender.UserId,
                ReceiverId = receiver.UserId,
                SenderUserName = sender.UserName,
                ReceiverUserName = request.ReceiverUserName,
                IsAccepted = false,
                SentAt = DateTime.UtcNow
            };

            var result = await _friendRequestRepository.AddFriendRequestAsync(friendRequest);

            if (!result)
            {
                return new SendFriendRequestResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Failed to send friend request."
                };
            }

            return new SendFriendRequestResponseDto
            {
                IsSuccess = true,
                Message = "Friend request sent successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error sending friend request from email {Sender} to userName {Receiver}", request.SenderEmail, request.ReceiverUserName);
            return new SendFriendRequestResponseDto
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while sending the friend request."
            };
        }
    }
}