using api.Extensions;
using api.Helpers;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class PlayerController(IPlayerRepository _playerRepository,
IFollowRepository _followRepository, ITokenService _tokenService) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll([FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        PagedList<RootModel> pagedPlayers = await _playerRepository.GetAllAsync(paginationParams, cancellationToken);

        if (pagedPlayers.Count == 0)
            return NoContent(); 

        PaginationHeader paginationHeader = new(
            CurrentPage: pagedPlayers.CurrentPage,
            ItemsPerPage: pagedPlayers.PageSize,
            TotalItems: pagedPlayers.TotalItems,
            TotalPages: pagedPlayers.TotalPages
        );

        Response.AddPaginationHeader(paginationHeader);

        List<PlayerDto> playerDtos = [];

        string? playerIdHashed = User.GetHashedUserId();

        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(playerIdHashed, cancellationToken);

        if (playerId is null)
            return Unauthorized("You are not logged in. Login again");

        bool isFollowing;
        foreach (RootModel rootModel in pagedPlayers)
        {
            isFollowing = await _followRepository.CheckIsFollowingAsync(playerId.Value, rootModel, cancellationToken);

            playerDtos.Add(Mappers.ConvertRootModelToPlayerDto(rootModel, isFollowing));
        }

        return playerDtos;
    }

    [HttpGet("get-by-id/{playerId}")]
    public async Task<ActionResult<PlayerDto>> GetById(string playerId, CancellationToken cancellationToken)
    {
        PlayerDto? playerDto = await _playerRepository.GetByIdAsync(playerId, cancellationToken);

        if (playerDto is null)
            return NotFound("No player with this ID");

        return playerDto;
    }

    [HttpGet("get-by-username/{playerUserName}")]
    public async Task<ActionResult<PlayerDto>> GetByUserName(string playerUserName, CancellationToken cancellationToken)
    {
        PlayerDto? playerDto = await _playerRepository.GetByUserNameAsync(playerUserName, cancellationToken);

        if (playerDto is null)
            return NotFound("No Player with this email address");

        return playerDto;
    }
}
// test