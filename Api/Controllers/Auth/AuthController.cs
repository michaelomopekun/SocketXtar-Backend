using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.Dtos;
using Application.Users.Common.Exceptions;
using Application.Users.Dtos;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers.Auth;

public class AuthController : ControllerBase
{
    private readonly IDistributedCache _redis;
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger, IDistributedCache redis, IMediator mediator)
    {
        _redis = redis; 
        _logger = logger;
        _mediator = mediator;
    }
    

    [HttpPost]
    [Route("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyEmail([FromQuery] EmailVerificationRequest request)
    {
        try
        {
            var response = await _mediator.Send(request);

            return Ok(new { status = "success", data = response });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "❌user does not exist");

            return BadRequest(new { status = "error", message = ex.Message });
        }
        catch (InvalidOrExpiredTokenException ex)
        {
            _logger.LogError(ex, "❌token is invalid or empty");

            return BadRequest(new { status = "error", message = ex.Message });
        }
        catch (NullReferenceException ex)
        {
            _logger.LogError(ex, "❌email or token is empty");

            return BadRequest(new { status = "error", message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "==========❌Error while processing email verification==========");

            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }


    // [Authorize]
    [HttpPost]
    [Route("onboard")]
    public async Task<IActionResult> Onboard([FromBody] RegisterUserRequest request)
    {
        try
        {
            var result = await _mediator.Send(request);
            return Ok(new { status = "success", data = result });
        }
        catch (NullReferenceException ex)
        {
            _logger.LogError(ex, $"❌value is empty");
            return BadRequest(new { status = "error", message = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, $"❌Argument request is empty");
            return BadRequest(new { status = "error", message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, $"❌Unauthorized access attempt by User {request.Email}");
            return Unauthorized(new { status = "error", message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌error onboarding User {request.Email}");
            return StatusCode(500, new { status = "error", message = ex.Message });
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
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, $"Unauthorized access attempt by User {request.Email}");
            return Unauthorized(new { status = "error", message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"error logging in User {request.Email}");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }


    [Authorize]
    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new { status = "error", message = "Missing token, can't invalidate session without token." });
            }

            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(token) as JwtSecurityToken;

            var expires = jwtToken?.ValidTo ?? DateTime.UtcNow.AddHours(2);

            var ttl = expires - DateTime.UtcNow;

            await _redis.SetStringAsync($"blacklist:{token}", "revoked", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            });

            _logger.LogInformation($"✅User logged out successfully. Token: {token} will be blacklisted for {ttl.TotalMinutes} minutes.");

            return Ok(new { status = "success", message = "User logged out successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error logging out user.");

            return StatusCode(500, new { status = "error", message = "Error logging out user.", errorMessage = ex.Message });
        }
    }

}