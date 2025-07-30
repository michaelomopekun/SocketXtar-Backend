using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IFriendRequestRepository
{
    public Task<IEnumerable<FriendRequest>?> GetFriendRequestsBySenderUsernameAsync(string SenderUserName);
    public Task<IEnumerable<FriendRequest>?> GetFriendRequestsByReceiverUsernameAsync(string ReceiverUserName);
    public Task<FriendRequest?> GetFriendRequestByIdAsync(Guid id);
    public Task<bool> AddFriendRequestAsync(FriendRequest friendRequest);
    public Task<bool> DeleteFriendRequestByIdAsync(Guid id);
    public Task<bool> DeleteFriendRequestBySenderAndReceiverUsernameAsync(string senderUserName, string receiverUserName);
}
