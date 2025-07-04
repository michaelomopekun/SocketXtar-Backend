using Domain.Entities;
namespace Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<bool> UserExistByUsernameAsync(string username);
    Task AddUserAsync(User user);
    Task SaveChangesAsync();
}

