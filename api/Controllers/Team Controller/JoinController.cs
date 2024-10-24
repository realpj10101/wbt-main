using api.DTOs.Team;
using api.Extensions;
using api.Interfaces.Team;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using SharpCompress.Archives.Tar;

namespace api.Controllers;

[Authorize]
public class JoinController(IJoinRepository _joinRepository, ITokenService _tokenService) : BaseApiController
{
    [HttpPost("add-join/{targetMemeberUserName}")]
    public async Task<ActionResult<Response>> Create(string targetTeamUserName, CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (playerId is null)
            return Unauthorized("You are not logged in, Login again.");

        JoinStatus joinStatus = await _joinRepository.CreateJoinAsync(playerId.Value, targetTeamUserName, cancellationToken);

        return joinStatus.IsSuccess
        ? Ok(new Response(Message: $"You joined {targetTeamUserName} successfully."))
        : joinStatus.IsTargetTeamNotFound
        ? NotFound($"{targetTeamUserName} is not found.")
        : joinStatus.IsJoiningThemself
        ? BadRequest("Joining youself is good but not stored.")
        : joinStatus.IsAlreadyJoined
        ? BadRequest($"You have already joined {targetTeamUserName}.")
        : BadRequest("Joining failed. Try again or cantact support.");
    }

    [HttpGet("remove-join/{targetTeamUserName}")]
    public async Task<ActionResult<Response>> Remove(string targetTeamUserName, CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (playerId is null)
            return Unauthorized("You are not logged in");

        JoinStatus jS = await _joinRepository.RemoveAsync(playerId.Value, targetTeamUserName, cancellationToken);

        return jS.IsSuccess
        ? Ok(new Response(Message: $"You left {targetTeamUserName} successfuly."))
        : jS.IsTargetTeamNotFound
        ? NotFound($"{targetTeamUserName} not found.")
        : jS.IsAlreadyLeft
        ? BadRequest($"You already left{targetTeamUserName}")
        : BadRequest("Left failed. Try again or contact support.");
    }

    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<TeamDto>>>
}