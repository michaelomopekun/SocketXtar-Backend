using DnsClient.Internal;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;

namespace Infrastructure.Repositories;

public class FriendRequestRepository : IFriendRequestRepository
{
    private readonly ChatAppDBContext _context;
    private readonly ILogger<FriendRequestRepository> _logger;

    public FriendRequestRepository(ChatAppDBContext context, ILogger<FriendRequestRepository> logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task<bool> AddFriendRequestAsync(FriendRequest friendRequest)
    {
        try
        {
            await _context.FriendRequests.AddAsync(friendRequest);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error adding friend request");
            return false;
        }
    }

    public async Task<bool> DeleteFriendRequestByIdAsync(Guid id)
    {
        try
        {
            var friendRequest = await _context.FriendRequests.FindAsync(id);
            if (friendRequest == null) return false;

            _context.FriendRequests.Remove(friendRequest);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error deleting friend request by ID");
            return false;
        }
    }

    public async Task<bool> DeleteFriendRequestBySenderAndReceiverUsernameAsync(string senderUserName, string receiverUserName)
    {
        try
        {
            var friendRequest = await _context.FriendRequests.FirstOrDefaultAsync(fr => fr.SenderUserName == senderUserName && fr.ReceiverUserName == receiverUserName);

            if (friendRequest == null) return false;

            _context.FriendRequests.Remove(friendRequest);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error deleting friend request by sender and receiver username");
            return false;
        }
    }

    public async Task<FriendRequest> GetFriendRequestBySenderAndReceiverUsernameAsync(string senderUserName, string receiverUserName)
    {
        try
        {
            var friendRequest = await _context.FriendRequests.FirstOrDefaultAsync(fr => fr.SenderUserName == senderUserName && fr.ReceiverUserName == receiverUserName);

            if (friendRequest == null) return null!;

            return friendRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error fetching friend request by sender and receiver username");
            return null!;
        }
    }

    public async Task<FriendRequest?> GetFriendRequestByIdAsync(Guid id)
    {
        try
        {
            return await _context.FriendRequests.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error getting friend request by ID");
            return null;
        }
    }

    public async Task<IEnumerable<FriendRequest>?> GetFriendRequestsByReceiverUsernameAsync(string ReceiverUserName)
    {
        try
        {
            var requests = await _context.FriendRequests.Where(u => u.ReceiverUserName == ReceiverUserName).ToListAsync();

            return requests;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error getting friend requests by receiver username");
            return null;
        }
    }

    public async Task<IEnumerable<FriendRequest>?> GetFriendRequestsBySenderUsernameAsync(string SenderUserName)
    {
        try
        {
            var requests = await _context.FriendRequests.Where(u => u.SenderUserName == SenderUserName).ToListAsync();

            return requests;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error getting friend requests by sender username");
            return null;
        }
    }

}