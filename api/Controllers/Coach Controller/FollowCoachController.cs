using api.Extensions;
using api.Helpers;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class FollowCoachController(IFollowCoachRepository _followCoachRepository, ITokenService _tokenService) : BaseApiController
{
    [HttpPost("add-coach-follow/{targetMemberUserName}")]
    public async Task<ActionResult<Response>> Create(string targetMemberUserName, CancellationToken cancellationToken)
    {
        ObjectId? coachId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (coachId is null)
            return Unauthorized("You are not logged in. Login again");

        FollowStatus followCoStatus = await _followCoachRepository.CreateFollowCoAsync(coachId.Value, targetMemberUserName, cancellationToken);

        return followCoStatus.IsSuccess
        ? Ok(new Response(Message: $"You followed {targetMemberUserName} successfully"))
        : followCoStatus.IsTargetMemberNotFound
        ? NotFound($"{targetMemberUserName} is not found")
        : followCoStatus.IsFollowingThemself
        ? BadRequest("Following yourself is great  but not stored")
        : followCoStatus.IsAlreadyFollowed
        ? BadRequest($"{targetMemberUserName} is already followed")
        : BadRequest("Following failed. Try again or contact support");
    }


  [HttpDelete("remove-follow/{targetMemberUserName}")]
  public async Task<ActionResult<Response>> Remove(string targetMemberUserName, CancellationToken cancellationToken)
  {
    ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

    if (playerId is null)
       return Unauthorized("You are not logged in. login again.");

    FollowStatus fS = await _followCoachRepository.RemoveAsync(playerId.Value, targetMemberUserName, cancellationToken);

    return fS.IsSuccess
    ? Ok(new Response(Message: $"You UnFollowed {targetMemberUserName} successfuly."))
    : fS.IsTargetMemberNotFound
    ? NotFound($"{targetMemberUserName} is not found.")
    : fS.IsAlreadyUnfollowed
    ? BadRequest($"{targetMemberUserName} is already UnFollowed.")
    : BadRequest("UnFollowing failed. Try again or contact support.");
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<CoachDto>>> GetAll([FromQuery] FollowParams followParams, CancellationToken cancellationToken)
  {
    ObjectId? coachId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

    if (coachId is null)
      return Unauthorized("You are not logged in. Login again.");

    followParams.UserId = coachId.Value;

    PagedList<RootModel> pagedRootModels = await _followCoachRepository.GetAllAsync(followParams, cancellationToken);

    if (pagedRootModels.Count == 0)
      return NoContent();

    Response.AddPaginationHeader(new (
      pagedRootModels.CurrentPage,
      pagedRootModels.PageSize,
      pagedRootModels.TotalItems,
      pagedRootModels.TotalPages
    ));

    List<CoachDto> coachDtos = [];

    bool isFollowing;
    if (pagedRootModels is not null)
      foreach (RootModel rootModel in pagedRootModels)
      {
        isFollowing = await _followCoachRepository.CheckIsFollowingAsync(coachId.Value, rootModel, cancellationToken);

        coachDtos.Add(CoachMappers.ConvertRootModelToCoachDto(rootModel, isFollowing));
      }

      return coachDtos;
  }
}