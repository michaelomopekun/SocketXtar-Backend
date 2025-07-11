using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Users.Common.Interface;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    public string GenerateToken(User user)
    {
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new NullReferenceException("env var JWT_SECRET cant be found");

        var key = Encoding.ASCII.GetBytes(secret);

        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("username", user.UserName),
            new Claim("role", user.UserRole.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

}
