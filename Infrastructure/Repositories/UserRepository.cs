using Domain.Entities;
using Persistence.Data;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ChatAppDBContext _context;

    public UserRepository(ChatAppDBContext context)
    {
        _context = context;
    }


    public async Task AddUserAsync(User user)
    {
        await _context.User.AddAsync(user);

        await SaveChangesAsync();
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

    public async Task<bool> UserExistByUsernameAsync(string username)
    {
        return await _context.User.AnyAsync(n => username == n.UserName);
    }

}