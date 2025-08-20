using Application.Users.Dtos.FriendRequest;
using MediatR;

namespace Application.Users.Queries;

public record GetAllFriendRequestQueries(string userEmail): IRequest<List<FriendRequestDTO>>;
