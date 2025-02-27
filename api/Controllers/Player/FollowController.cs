using api.Extensions;
using api.Helpers;
using api.Interfaces.Player;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers.Player;

[Authorize]
public class FollowController(
    IFollowRepository _followRepository, ILikeRepository _likeRepository,
    ITokenService _tokenService) : BaseApiController
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

    [HttpDelete("remove-follow/{targetPlayerUserName}")]
    public async Task<ActionResult<Response>> Delete(string targetPlayerUserName, CancellationToken cancellationToken)
    {
        // Get logged in user ObjectId from token.
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken); 
        
        if (playerId is null)
            return Unauthorized("You are not logged in. Please login again.");
                
        FollowStatus fS = await _followRepository.DeleteAsync(playerId.Value, targetPlayerUserName, cancellationToken);
        
        return fS.IsSuccess
        ? Ok(new Response(Message: $"You unfollowed {targetPlayerUserName} successfully."))
        : fS.IsTargetMemberNotFound
        ? NotFound($"{targetPlayerUserName} is not found.")
        : fS.IsAlreadyUnfollowed
        ? BadRequest($"{targetPlayerUserName} is already unfollowed.")
        : BadRequest("Unfollowing failed. Please try again or contact the administrator.");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAllAsync([FromQuery] FollowParams followParams,
        CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (playerId is null)
            return Unauthorized("You are not logged in. Please login again.");

        followParams.UserId = playerId;

        PagedList<AppUser> pagedAppUsers = await _followRepository.GetAllAsync(followParams, cancellationToken);

        if (pagedAppUsers.Count == 0) return NoContent();
        
        Response.AddPaginationHeader(new (
            pagedAppUsers.CurrentPage,
            pagedAppUsers.PageSize,
            pagedAppUsers.TotalItems,
            pagedAppUsers.TotalPages
            ));

        List<PlayerDto> playerDtos = [];

        bool isFollowing;
        bool isLiking;
        foreach (AppUser appUser in pagedAppUsers)
        {
            isFollowing = await _followRepository.CheckIsFollowingAsync(playerId.Value, appUser, cancellationToken);
            
            isLiking = await _likeRepository.CheckIsLikingAsync(playerId.Value, appUser, cancellationToken);
            
            playerDtos.Add(Mappers.ConvertAppUserToPlayerDto(appUser, isFollowing, isLiking));
        }

        return playerDtos;
    }
}