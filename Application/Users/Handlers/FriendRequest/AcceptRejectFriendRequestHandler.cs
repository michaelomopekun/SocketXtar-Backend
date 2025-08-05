using Application.Users.Commands.FriendRequest;
using Application.Users.Common.Exceptions;
using Application.Users.Dtos.FriendRequest;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers.FriendRequest;

public class AcceptRejectFriendRequestHandler : IRequestHandler<AcceptRejectFriendRequestCommand, FriendRequestDecisionResponseDTO>
{
    
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly ILogger<AcceptRejectFriendRequestHandler> _logger;
    private readonly IUserContextService _userContextService;

    public AcceptRejectFriendRequestHandler(IFriendRequestRepository friendRequestRepository, ILogger<AcceptRejectFriendRequestHandler> logger, IUserContextService userContextService)
    {
        _friendRequestRepository = friendRequestRepository;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task<FriendRequestDecisionResponseDTO> Handle(AcceptRejectFriendRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Decision == null) throw new ArgumentNullException(nameof(request.Decision), "Decision cannot be null");

            var userEmail = _userContextService.GetUserEmailClaim();

            var friendRequest = await _friendRequestRepository.GetFriendRequestByIdAsync(request.Decision.RequestId);

            if (friendRequest == null) throw new NotFoundException("Friend request does not exist");

            if (request.Decision.IsAccepted)
            {
                friendRequest.IsAccepted = RequestStatus.Accepted;
                friendRequest.AcceptedAt = DateTime.UtcNow;
            }
            else if (!request.Decision.IsAccepted)
            {
                friendRequest.IsAccepted = RequestStatus.Rejected;
                friendRequest.RejectedAt = DateTime.UtcNow;
            }

            var updateResult = await _friendRequestRepository.UpdateFriendRequestAsync(friendRequest);

            if (!updateResult)
            {
                throw new Exception("Failed to update friend request status");
            }

            _logger.LogInformation("Friend request {RequestId} has been {Decision} by {UserEmail}", request.Decision.RequestId, request.Decision.IsAccepted ? "accepted" : "rejected", userEmail);

            return new FriendRequestDecisionResponseDTO
            {
                RequestId = request.Decision.RequestId,
                IsAccepted = request.Decision.IsAccepted,
                DecisionTime = request.Decision.IsAccepted ? friendRequest.AcceptedAt ?? DateTime.UtcNow : friendRequest.RejectedAt ?? DateTime.UtcNow,
                Message = request.Decision.IsAccepted ? "Friend request accepted successfully." : "Friend request rejected"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error processing friend request decision for {RequestId}", request.Decision?.RequestId);
            throw new Exception("An error occurred while processing the friend request decision", ex);
        }
    }

}
