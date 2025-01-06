using api.Extensions;
using api.Helpers;
using api.Interfaces.Player;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers.Player;

[Authorize]
public class MemberController(IMemberRepository _memberRepository, ITokenService _tokenService) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll([FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        PagedList<AppUser> pagedAppUsers = await _memberRepository.GetAllAsync(paginationParams, cancellationToken);

        if (pagedAppUsers.Count == 0)
            return NoContent();

        PaginationHeader paginationHeader = new(
                CurrentPage: pagedAppUsers.CurrentPage,
                ItemsPerPage: pagedAppUsers.PageSize,
                TotalItems: pagedAppUsers.TotalItems,
                TotalPages: pagedAppUsers.TotalPages
            );
        
        Response.AddPaginationHeader(paginationHeader);

        string? userIdHashed = User.GetHashedUserId();

        ObjectId? userId = await _tokenService.GetActualUserIdAsync(userIdHashed, cancellationToken);

        if (userId is null) return Unauthorized("You are not logged in! Login again.");

        List<PlayerDto> playerDtos = [];

        foreach (AppUser appUser in pagedAppUsers)
        {
            playerDtos.Add(Mappers.ConvertAppUserToPlayerDto(appUser));
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
        PlayerDto? playerDto = await _memberRepository.GetByUserNameAsync(playerUserName, cancellationToken);
        
        if (playerDto is null) return NotFound("No player with this username");

        return playerDto;
    }
}