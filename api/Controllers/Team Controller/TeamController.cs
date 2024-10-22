using api.DTOs.Team;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Team;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class TeamController(ITeamRepository _teamRepository,
IJoinRepository _joinRepository, ITokenService _tokenService) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetAll([FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        PagedList<RootModel> pagedTeams = await _teamRepository.GetAllAsync(paginationParams, cancellationToken);

        if (pagedTeams.Count == 0)
            return NoContent();

        PaginationHeader paginationHeader = new(
            CurrentPage: pagedTeams.CurrentPage,
            ItemsPerPage: pagedTeams.PageSize,
            TotalItems: pagedTeams.TotalItems,
            TotalPages: pagedTeams.TotalPages
        );

        Response.AddPaginationHeader(paginationHeader);

        List<TeamDto> teamDtos = [];

        string? teamIdHashed = User.GetHashedUserId();

        ObjectId? teamId = await _tokenService.GetActualUserIdAsync(teamIdHashed, cancellationToken);

        if (teamId is null)
            return Unauthorized("You are not logged in. Login again");

        bool isJoining;
        foreach (RootModel rootModel in pagedTeams)
        {
            isJoining = await _joinRepository.CheckIsJoiningAsync(teamId.Value, rootModel, cancellationToken);

            teamDtos.Add(TeamMappers.ConvertRootModelToTeamDto(rootModel, isJoining));
        }

        return teamDtos;
    }

    [HttpGet("get-by-id/{teamId}")]
    public async Task<ActionResult<TeamDto>> GetById(string teamId, CancellationToken cancellationToken)
    {
        TeamDto? teamDto = await _teamRepository.GetByIdAsync(teamId, cancellationToken);

        if (teamDto is null)
            return NotFound("No team with this ID");

        return teamDto;
    }

    [HttpGet("get-by-username/{teamUserName}")]
    public async Task<ActionResult<TeamDto>> GetByUserName(string teamUserName, CancellationToken cancellationToken)
    {
        TeamDto? teamDto = await _teamRepository.GetByUserNameAsync(teamUserName, cancellationToken);

        if (teamDto is null)
           return NotFound("No team with this email address");

        return teamDto;
    }
}
