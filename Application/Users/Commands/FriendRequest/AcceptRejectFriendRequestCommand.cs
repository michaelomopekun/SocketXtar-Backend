using Application.Users.Dtos.FriendRequest;
using MediatR;

namespace Application.Users.Commands.FriendRequest;

public record AcceptRejectFriendRequestCommand(FriendRequestDecisionDTO Decision): IRequest<FriendRequestDecisionDTO>;
