using Domain.Entities;

namespace Application.Users.Common.Interface;

public interface ITokenService
{
    string GenerateToken(User user);
}

