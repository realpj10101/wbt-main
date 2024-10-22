using api.Extensions;
using api.Helpers;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class FollowController(IFollowRepository _followRepository, ITokenService _tokenService) : BaseApiController
{
  [HttpPost("add-follow/{targetMemeberUserName}")]
  public async Task<ActionResult<Response>> Create(string targetMemberUserName, CancellationToken cancellationToken)
  {
    ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

    if (playerId is null)
      return Unauthorized("You are not logged in, Login again");

    FollowStatus followStatus = await _followRepository.CreateFollowAsync(playerId.Value, targetMemberUserName, cancellationToken);

    return followStatus.IsSuccess
    ? Ok(new Response(Message: $"You followed {targetMemberUserName} successfully"))
    : followStatus.IsTargetMemberNotFound
    ? NotFound($"{targetMemberUserName} is not found.")
    : followStatus.IsFollowingThemself
    ? BadRequest("Following yourself is good but not stored.")
    : followStatus.IsAlreadyFollowed
    ? BadRequest($"{targetMemberUserName} is already followed.")
    : BadRequest("Following failed. Try again or contact support.");
  }

  [HttpDelete("remove-follow/{targetMemberUserName}")]
  public async Task<ActionResult<Response>> Remove(string targetMemberUserName, CancellationToken cancellationToken)
  {
    ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

    if (playerId is null)
      return Unauthorized("You are not logged in. login again.");

    FollowStatus fS = await _followRepository.RemoveAsync(playerId.Value, targetMemberUserName, cancellationToken);

    return fS.IsSuccess
    ? Ok(new Response(Message: $"You UnFollowed {targetMemberUserName} successfuly."))
    : fS.IsTargetMemberNotFound
    ? NotFound($"{targetMemberUserName} is not found.")
    : fS.IsAlreadyUnfollowed
    ? BadRequest($"{targetMemberUserName} is already UnFollowed.")
    : BadRequest("UnFollowing failed. Try again or contact support.");
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll([FromQuery] FollowParams followParams, CancellationToken cancellationToken)
  {
    ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

    if (playerId is null)
      return Unauthorized("You are not logged in. Login again.");

    followParams.UserId = playerId.Value;

    PagedList<RootModel> pagedRootModels = await _followRepository.GetAllAsync(followParams, cancellationToken);

    if (pagedRootModels.Count == 0)
      return NoContent();

    Response.AddPaginationHeader(new(
      pagedRootModels.CurrentPage,
      pagedRootModels.PageSize,
      pagedRootModels.TotalItems,
      pagedRootModels.TotalPages
      ));

    List<PlayerDto> playerDtos = [];

    bool isFollowing;
    if (pagedRootModels is not null)
      foreach (RootModel rootModel in pagedRootModels)
      {
        isFollowing = await _followRepository.CheckIsFollowingAsync(playerId.Value, rootModel, cancellationToken);

        playerDtos.Add(Mappers.ConvertRootModelToPlayerDto(rootModel, isFollowing));
      }

    return playerDtos;
  }
}