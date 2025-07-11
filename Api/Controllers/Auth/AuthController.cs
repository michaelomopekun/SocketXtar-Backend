using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.Dtos;
using Application.Users.Dtos;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers.Auth;

public class AuthController : ControllerBase
{
    private readonly IDistributedCache _redis;
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;
    private readonly IUserRepository _userRepository;

    public AuthController(ILogger<AuthController> logger, IDistributedCache redis, IUserRepository userRepository, IMediator mediator)
    {
        _redis = redis;
        _logger = logger;
        _mediator = mediator;
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

            if (storedToken is null || storedToken != token) return BadRequest(new { status = "error", message = "Invalid or expired token." });

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


    [HttpPost]
    [Route("onboard")]
    public async Task<IActionResult> Onboard([FromBody] RegisterUserRequest request)
    {
        try
        {
            var result = await _mediator.Send(request);
            return Ok(new { status = "success", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"error onboarding User {request.Email}");
            return StatusCode(500, new { status = "error", message = $"Onboard :endpoint: Error Onboarding user {request.Email}." });
        }
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        try
        {
            var result = await _mediator.Send(request);
            return Ok(new { status = "success", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"error logging in User {request.Email}");
            return StatusCode(500, new { status = "error", message = $"Login :endpoint: Error logging in user {request.Email}." });
        }
    }
}
