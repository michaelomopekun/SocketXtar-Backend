using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;

namespace Infrastructure.Repositories;

public class FriendRepository : IFriendRepository
{

    private readonly ChatAppDBContext _content;
    private readonly ILogger<FriendRepository> _logger;

    public FriendRepository(ChatAppDBContext content, ILogger<FriendRepository> logger)
    {
        _content = content;
        _logger = logger;
    }

    public async Task<bool> AddFriendAsync(Friend friend1, Friend friend2)
    {
        if (friend1 == null || friend2 == null) throw new ArgumentNullException(nameof(friend1), "Friend1 or Friend2 cannot be null");

        try
        {
            for (int i = 0; i <= 1; i++)
            {
                var friend = i == 0 ? friend1 : friend2;

                _content.Friend.Add(friend);

                await _content.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding friends: {Message}", ex.Message);

            return false;
        }
    }
    

    public Task<Friend?> GetFriendByUsernamesAsync(string userName, string friendUserName)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(friendUserName))
            throw new ArgumentException("UserName and FriendUserName cannot be null or empty");

        return _content.Friend
            .FirstOrDefaultAsync(f => f.UserName == userName && f.FriendUserName == friendUserName);
    }

    public async Task<IEnumerable<Friend>> GetFriendsByUsernameAsync(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            throw new ArgumentException("UserName cannot be null or empty");

        return await _content.Friend
            .Where(f => f.UserName == userName)
            .ToListAsync();
    }

    public async Task<bool> RemoveFriendAsync(string userName, string friendUserName)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(friendUserName))
            throw new ArgumentException("UserName and FriendUserName cannot be null or empty");

        var friend = await _content.Friend
            .FirstOrDefaultAsync(f => f.UserName == userName && f.FriendUserName == friendUserName);

        if (friend == null) return false;

        _content.Friend.Remove(friend);
        await _content.SaveChangesAsync();

        return true;
    }

}
