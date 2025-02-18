using api.DTOs.Team_DTOs;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Teams;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api.Controllers.Teams;

[Authorize]
public class TeamController(ITeamRepository _teamRepository, ITokenService _tokenService) : BaseApiController
{
    [HttpPost("create")]
    public async Task<ActionResult<ShowTeamDto>> Create(CreateTeamDto userInput, CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);
        
        if (userId is null) return Unauthorized("You are not logged in. Please login again.");

        ShowTeamDto? showTeamDto = await _teamRepository.CreateAsync(userId.Value, userInput, cancellationToken);
        
        return showTeamDto is not null
            ? Ok(showTeamDto)
            : showTeamDto is null
            ? BadRequest("Team is already exists")
            : BadRequest("Create team failed. try again or contact administrator.");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShowTeamDto>>> GetAll([FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        PagedList<Team>? pagedTeams = await _teamRepository.GetAllAsync(paginationParams, cancellationToken);

        if (pagedTeams.Count == 0)
            return NoContent();
        
        PaginationHeader paginationHeader = new(
            CurrentPage: pagedTeams.CurrentPage,
            ItemsPerPage: pagedTeams.PageSize,
            TotalItems: pagedTeams.TotalItems,
            TotalPages: pagedTeams.TotalPages
            );
        
        Response.AddPaginationHeader(paginationHeader);

        string? userIdHashed = User.GetHashedUserId();

        ObjectId? userId = await _tokenService.GetActualUserIdAsync(userIdHashed, cancellationToken);
        
        if (userId is null) return Unauthorized("You are not logged in. Please login again.");
        
    }

    [HttpPut("update-team/{teamName}")]
    public async Task<ActionResult> Update(UpdateTeamDto userInput, string teamName, CancellationToken cancellationToken)
    {
        UpdateResult? updateRes = await _teamRepository.UpdateTeamAsync(userInput, teamName, cancellationToken);
        
        return updateRes is null
            ? BadRequest("Username is already exists.")
            : !updateRes.IsModifiedCountAvailable
            ? BadRequest("Update failed. Try again later.")
            : Ok("Team has been updated successfully.");

        // return updateRes is null || !updateRes.IsModifiedCountAvailable
        //     ? BadRequest("Update failed. Try again later.")
        //     : Ok(new { message = "Team has been updated successfully." });
    }
}       