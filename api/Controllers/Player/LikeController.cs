using api.Extensions;
using api.Interfaces.Player;
using api.Models.Helpers;

namespace api.Controllers.Player;

public class LikeController(ILikeRepository _likeRepository, ITokenService _tokenService) : BaseApiController
{
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
}