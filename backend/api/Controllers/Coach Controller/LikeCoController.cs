using System.Security.AccessControl;
using api.Extensions;
using api.Models.Helpers;

namespace api.Controllers;

public class LikeCoController(ILikeCoachRepository _likeCoachRepository, ITokenService _tokenService) : BaseApiController
{
    [HttpPost("add-like/{targetMemberUserName}")]
    public async Task<ActionResult<Response>> CreateLike(string targetMemberUserName, CancellationToken cancellationToken )
    {
        ObjectId? coachId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (coachId is null)
            return Unauthorized("You are not logged in, login again");

        LikeStatus likeStatus = await _likeCoachRepository.CreateLikeCoAsync(coachId.Value, targetMemberUserName, cancellationToken);

        return likeStatus.IsSuccess
        ? Ok(new Response(Message: $"{targetMemberUserName} is liked succesfully."))
        : likeStatus.IsTargetMemberNotFound
        ? NotFound($"{targetMemberUserName} is not found.")
        :likeStatus.IsLikingThemself
        ? BadRequest("Liking yourself is good but not stored")
        : likeStatus.IsAlreadyLiked
        ? BadRequest($"{targetMemberUserName} is already liked")
        : BadRequest("Liking failed. Try again or conntact support");
    }


    [HttpDelete("remove-like/{targetMemberUserName}")]
    public async Task<ActionResult<Response>> RemoveLike(string targetMemberUserName, CancellationToken cancellationToken)
    {
        ObjectId? coachId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (coachId is null)
            return Unauthorized("You are not logged in. login again");

        LikeStatus lS = await _likeCoachRepository.RemoveLikeCoAsync(coachId.Value, targetMemberUserName, cancellationToken);

        return lS.IsSuccess
        ? Ok(new Response(Message: $"You disLiked{targetMemberUserName} successfully."))
        : lS.IsTargetMemberNotFound
        ? NotFound($"{targetMemberUserName} is not found.")
        : lS.IsAlreadyDisLiked
        ? BadRequest($"{targetMemberUserName} is already UnFollowed.")
        : BadRequest("UnFollowing failed. Try again or contact support");
    }
}