using Application.Users.Common.Exceptions;
using Application.Users.Dtos.FriendRequest;
using Application.Users.Queries;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers.FriendRequest;

public class GetAllFriendRequestHandler : IRequestHandler<GetAllFriendRequestQueries, List<FriendRequestDTO>>
{
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllFriendRequestHandler> _logger;

    public GetAllFriendRequestHandler(IFriendRequestRepository friendRequestRepository, IUserRepository userRepository, ILogger<GetAllFriendRequestHandler> logger)
    {
        _friendRequestRepository = friendRequestRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<List<FriendRequestDTO>> Handle(GetAllFriendRequestQueries request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.userEmail)) throw new ArgumentNullException("email is null or empty");

        var user = await _userRepository.GetUserByEmailAsync(request.userEmail) ?? throw new NotFoundException("user those not Exist");

        var friendRequests = await _friendRequestRepository.GetFriendRequestsByReceiverUsernameAsync(user.UserName) ?? throw new Exception("user does not have a friend request");

        // var friendRequest = new FriendRequestDTO
        // {

        // };
        
        return (List<FriendRequestDTO>)friendRequests;



    }

}
