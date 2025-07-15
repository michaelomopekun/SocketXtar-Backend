using System.Security.Claims;
using Application.Dtos;
using Application.Users.Common.Exceptions;
using Application.Users.Common.Profile;
using Application.Users.Dtos.Profile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Profile;

public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProfileController> _logger;
    private string GetUserEmailClaim() => User.FindFirst(ClaimTypes.Email)?.Value
                                            ?? throw new UnauthorizedAccessException("User email not found in claims");

    public ProfileController(IMediator mediator, ILogger<ProfileController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    [Route("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserProfile()
    {
        
        try
        {
            var email = GetUserEmailClaim();
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("User email not found in claims");

                return Unauthorized(new { status = "error", message = "User email not found in claims" });
            }

            var profile = await _mediator.Send(new GetUserProfileCommand(email));
            if (profile == null)
            {
                _logger.LogWarning("❌User profile not found");

                return NotFound(new { status = "error", message = "User profile not found" });
            }

            _logger.LogInformation("✅User profile retrieved successfully for email: {Email}", email);

            return Ok(new { status = "success", data = profile });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "❌Cached User profile not found");

            return NotFound(new { status = "error", message = ex.Message });
        }
        catch (NullReferenceException ex)
        {
            _logger.LogError(ex, "❌User profile data is null");

            return BadRequest(new { status = "error", message = "User profile data is null" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error fetching user profile");

            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }


    [Authorize]
    [HttpPatch]
    [Route("profileUpdate")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest body)
    {

        try
        {
            var email = GetUserEmailClaim();
            _logger.LogInformation("Updating user profile for email: {Email}", email);

            if (string.IsNullOrEmpty(email)) return Unauthorized(new { status = "error", message = "User email not found in claims" });

            var success = await _mediator.Send(new UpdateUserProfileCommand(email, body));

            if (!success) return NotFound(new { status = "error", message = "User not found or update failed." });

            return Ok(new { status = "success", message = "Profile updated successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error updating user profile");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }



    [Authorize]
    [HttpPost("profile/picture")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UploadProfilePictureResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadProfilePicture([FromForm] UploadProfilePictureRequest request)
    {
        try
        {
            var email = GetUserEmailClaim();
            _logger.LogInformation("Uploading profile picture for user {Email}", email);

            if (request.PictureFile == null || request.PictureFile.Length == 0)
                return BadRequest(new { status = "error", message = "No file provided." });

            if (string.IsNullOrEmpty(email)) return Unauthorized(new { status = "error", message = "User email not found in claims" });

            var response = await _mediator.Send(new UploadProfilePictureCommand(email, request.PictureFile));

            return Ok(new { status = "success", url = response.Url, message = response.Message });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "❌User not found for email {Email}", GetUserEmailClaim());
            return NotFound(new { status = "error", message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "❌Invalid file provided for user {Email}", GetUserEmailClaim());
            return BadRequest(new { status = "error", message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error uploading profile picture for user {Email}", GetUserEmailClaim());
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

}