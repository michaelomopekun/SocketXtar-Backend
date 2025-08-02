using Api.Controllers.Auth;
using Api.Controllers.Profile;
using Application.Users.Commands.FriendRequest;
using Application.Users.Dtos.FriendRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.FriendRequest;


[ApiController]
[Route("api/[controller]")]
public class FriendRequestController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FriendRequestController> _logger;
    private readonly IUserContextService _userContextService;

    public FriendRequestController(IMediator mediator, ILogger<FriendRequestController> logger, IUserContextService userContextService)
    {
        _mediator = mediator;
        _logger = logger;
        _userContextService = userContextService;
    }


    [HttpPost]
    [Authorize]
    [Route("send-friend-request")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendFriendRequest([FromBody] SendFriendRequestDTO request)
    {
        try
        {
            var email = _userContextService.GetUserEmailClaim();
            _logger.LogInformation("Sending friend request from {Email} to {TargetUserName}", email, request.ReceiverUserName);

            // if (string.IsNullOrEmpty(email)) return Unauthorized(new { status = "error", message = "User email not found in claims" });

            var result = await _mediator.Send(new SendFriendRequestCommand(email, request.ReceiverUserName));

            if (result.IsSuccess == false) return NotFound(new { status = "error", message = result.ErrorMessage });

            return Ok(new { status = "success", message = "Friend request sent successfully." });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "❌Unauthorized access while sending friend request");
            return Unauthorized(new { status = "error", message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌Error sending friend request");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

}
