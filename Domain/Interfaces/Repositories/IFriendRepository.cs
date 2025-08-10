using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IFriendRepository
{
    Task<IEnumerable<Friend>> GetFriendsByUsernameAsync(string userName);
    Task<Friend?> GetFriendByUsernamesAsync(string userName, string friendUserName);
    Task<bool> AddFriendAsync(Friend friend1, Friend friend2);
    Task<bool> RemoveFriendAsync(string userName, string friendUserName);
}
