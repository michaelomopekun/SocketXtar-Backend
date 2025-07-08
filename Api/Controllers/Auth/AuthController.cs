using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers.Auth;

public class AuthController : ControllerBase
{
    private readonly IDistributedCache _redis;
    private readonly ILogger<AuthController> _logger;
    private readonly IUserRepository _userRepository;

    public AuthController(ILogger<AuthController> logger, IDistributedCache redis, IUserRepository userRepository)
    {
        _logger = logger;
        _redis = redis;
        _userRepository = userRepository;
    }

    [HttpPost]
    [Route("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token)) return BadRequest(new { status = "error", message = "Missing email or token" });

            var storedToken = await _redis.GetStringAsync($"verify:{email}");

            if (storedToken is null || storedToken != token) return BadRequest("Invalid or expired token.");

            var handler = new JwtSecurityTokenHandler();

            var secret = Environment.GetEnvironmentVariable("EMAIL_SECRET") ?? throw new NullReferenceException("env var EMAIL_SECRET cant be found");

            var key = Encoding.ASCII.GetBytes(secret);

            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true
            }, out SecurityToken _);

            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null) return NotFound(new { status = "error", message = $"User: {email} not found" });

            user.EmailConfirmed = true;

            var update = await _userRepository.UpdateUserAsync(user);

            if (update == true)
            {
                await _redis.RemoveAsync($"verify:{email}");

                _logger.LogInformation("==========✅ Email verified successfully.==========");

                return Ok(new { status = "success", message = "Email verified successfully." });
            }

            return StatusCode(500, new { status = "error", message = "Failed to update email status." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "==========❌Error while processing email verification==========");

            return StatusCode(500, new { status = "error", message = "VERIFY EMAIL :endpoint: Error confirming email", errorMessage = ex.Message });
        }
    }


    // [HttpPost]
    // [Route("onboard")]
    // public Task<IActionResult> 
}
