using api.Extensions;
using api.Helpers;
using api.Interfaces.Player;
using api.Models.Helpers;

namespace api.Controllers.Player;

public class LikeController(
    ILikeRepository _likeRepository, 
    ITokenService _tokenService, IFollowRepository _followRepository) : BaseApiController
{
    // add like
    [HttpPost("add/{targetMemberUserName}")]
    public async Task<ActionResult<Response>> Create(string targetMemberUserName, CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (playerId is null)
            return Unauthorized("You are not logged in. Please login again.");

        LikeStatus lS = await _likeRepository.CreateAsync(playerId.Value, targetMemberUserName, cancellationToken);

        return lS.IsSuccess
            ? Ok(new Response(Message: $"You liked {targetMemberUserName} successfully."))
            : lS.IsTargetMemberNotFound
            ? NotFound($"{targetMemberUserName} was not found.")
            : lS.IsAlreadyLiked
            ? BadRequest($"{targetMemberUserName} is already liked.")
            : BadRequest("Liking failed. Please try again or contact the administrator.");
    }   
    
    // remove like
    [HttpDelete("remove/{targetMemberUserName}")]
    public async Task<ActionResult<Response>> Delete(string targetMemberUserName, CancellationToken cancellationToken)
    {
        // Get logged in user ObjectId from token.
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (playerId is null)
            return Unauthorized("You are not logged in. Please login again.");

        LikeStatus lS = await _likeRepository.DeleteAsync(playerId.Value, targetMemberUserName, cancellationToken);

        return lS.IsSuccess
            ? Ok(new Response(Message: $"You dislike {targetMemberUserName} successfully."))
            : lS.IsTargetMemberNotFound
            ? NotFound($"{targetMemberUserName} was not found.")
            : lS.IsAlreadyDisLiked
            ? BadRequest($"{targetMemberUserName} is already disliked.")
            : BadRequest("Disliking failed. Please try again or contact the administrator.");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll([FromQuery] LikeParams likeParams,
        CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (playerId is null)
            return Unauthorized("You are not logged in. Please login again.");

        likeParams.UserId = playerId;
        
        PagedList<AppUser> pagedAppUsers = await _likeRepository.GetAllAsync(likeParams, cancellationToken);

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