using api.Extensions;
using api.Helpers;
using api.Interfaces.Player;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace api.Controllers.Player;

[Authorize]
public class MemberController(IMemberRepository _memberRepository, 
    ITokenService _tokenService, IFollowRepository _followRepository,
    ILikeRepository _likeRepository, UserManager<AppUser> _userManager) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll([FromQuery] MemberParams memberParams,
        CancellationToken cancellationToken)
    {
        string? userIdHashed = User.GetHashedUserId();

        ObjectId? userId = await _tokenService.GetActualUserIdAsync(userIdHashed, cancellationToken);

        if (userId is null) return Unauthorized("You are not logged in! Login again.");

        memberParams.UserId = userId;
        
        PagedList<AppUser>? pagedAppUsers = await _memberRepository.GetAllAsync(memberParams, cancellationToken);

        if (pagedAppUsers.Count == 0)
            return NoContent();

        PaginationHeader paginationHeader = new(
                CurrentPage: pagedAppUsers.CurrentPage,
                ItemsPerPage: pagedAppUsers.PageSize,
                TotalItems: pagedAppUsers.TotalItems,
                TotalPages: pagedAppUsers.TotalPages
            );
        
        Response.AddPaginationHeader(paginationHeader);

        List<PlayerDto> playerDtos = [];

        bool isFollowing;
        bool isLiking;
        foreach (AppUser appUser in pagedAppUsers)
        {
            isFollowing = await _followRepository.CheckIsFollowingAsync(userId.Value, appUser, cancellationToken);

            isLiking = await _likeRepository.CheckIsLikingAsync(userId.Value, appUser, cancellationToken);
            
            playerDtos.Add(Mappers.ConvertAppUserToPlayerDto(appUser, isFollowing, isLiking));
        }

        return playerDtos;
    }

    [HttpGet("get-by-id/{playerId}")]
    public async Task<ActionResult<PlayerDto>> GetById(string playerId, CancellationToken cancellationToken)
    {
        PlayerDto? playerDto = await _memberRepository.GetByIdAsync(playerId, cancellationToken);
        
        if (playerDto is null) return NotFound("No player with this ID");

        return playerDto;
    }

    [HttpGet("get-by-username/{playerUserName}")]
    public async Task<ActionResult<PlayerDto>> GetByUsername(string playerUserName, CancellationToken cancellationToken)
    {
        string? userIdHashed = User.GetHashedUserId();

        if (userIdHashed is null) return Unauthorized("You are not logged in! Login again.");
        
        PlayerDto? playerDto = await _memberRepository.GetByUserNameAsync(playerUserName, userIdHashed, cancellationToken);
        
        if (playerDto is null) return NotFound("No player with this username");

        return playerDto;
    }
}