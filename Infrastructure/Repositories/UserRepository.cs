using Domain.Entities;
using Persistence.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces.Repositories;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ChatAppDBContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ChatAppDBContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task<bool> AddUserAsync(User user)
    {
        try
        {
            await _context.User.AddAsync(user);

            await SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "=========❌Failed to register user and save in DB=========");

            throw;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
            return await _context.User.FirstOrDefaultAsync(e => email == e.Email);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {    
        return await _context.User.FirstOrDefaultAsync(i => userId == i.UserId);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.User.FirstOrDefaultAsync(n => username == n.UserName);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        try
        {
            _context.User.Update(user);

            await SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "=========❌Failed to update user in DB==========");

            return false;
        }
    }


    public async Task<bool> UserExistByUsernameAsync(string username)
    {
        return await _context.User.AnyAsync(n => username == n.UserName);
    }

}