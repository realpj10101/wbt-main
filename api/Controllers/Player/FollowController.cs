using api.Extensions;
using api.Interfaces.Player;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api.Controllers.Player;

[Authorize]
public class FollowController(IFollowRepository _followRepository, ITokenService _tokenService) : BaseApiController
{
    [HttpPost("add-follow/{targetPlayerUserName}")]
    public async Task<ActionResult<Response>> Create(string targetPlayerUserName, CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);
        
        if (playerId is null)
            return Unauthorized("You are not logged in. Please login again.");
        
        FollowStatus fS = await _followRepository.CreateAsync(playerId.Value, targetPlayerUserName, cancellationToken);
        
        return fS.IsSuccess
            ? Ok(new Response(Message: $"You are now following {targetPlayerUserName} successfully."))
            : fS.IsTargetMemberNotFound
            ? NotFound($"{targetPlayerUserName} is not found.")
            : fS.IsFollowingThemself
            ? BadRequest("Follwing yourself is great but not stored.")
            : fS.IsAlreadyFollowed
            ? BadRequest($"{targetPlayerUserName} is already followed.")
            : BadRequest("Following failed. Please try again or contact the administrator.");
    }
}