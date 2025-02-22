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
        
        if ((userInput.GamesWon + userInput.GamesLost) > userInput.GamesPlayed)
            return BadRequest("Games won and games lost are more than games played.");

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

        List<ShowTeamDto> playerDtos = [];

        foreach (Team team in pagedTeams)
        {
            playerDtos.Add(Mappers.ConvertTeamToShowTeamDto(team));
        }

        return playerDtos;
    }

    [HttpPut("update-team/{teamName}")]
    public async Task<ActionResult<Response>> Update(UpdateTeamDto userInput, string teamName,
        CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);
        
        if (userId is null) 
            return Unauthorized("You are not logged in. Please login again.");

        TeamStatus? tS = await _teamRepository.UpdateTeamAsync(userId.Value, userInput, teamName, cancellationToken);
        
        return tS.IsSuccess
            ? Ok(new Response(Message: $"You updated the team {teamName} successfully."))
            : tS.IsTargetMemberNotFound
            ? NotFound($"{userInput.UserName} is not found.")
            : tS.IsTargetTeamNotFound
            ? NotFound($"Team {teamName} is not found.")
            : tS.IsAlreadyJoined
            ? BadRequest($"{userInput.UserName} is already joined.")
            : tS.IsJoiningThemself
            ? BadRequest("You are the owner of this team.")
            : BadRequest("Updated team failed. Please contact administrator.");
    }

    [HttpGet("get-by-name/{teamName}")]
    public async Task<ActionResult<ShowTeamDto>> GetByName(string teamName, CancellationToken cancellationToken)
    {
        ShowTeamDto? teamDto = await _teamRepository.GetByTeamNameAsync(teamName, cancellationToken);

        if (teamDto is null) return NotFound("No team with this Name was found.");

        return teamDto;
    }
    
    // old code
    // [HttpPut("update-team/{teamName}")]
    // public async Task<ActionResult> Update(UpdateTeamDto userInput, string teamName, CancellationToken cancellationToken)
    // {
    //     UpdateResult? updateRes = await _teamRepository.UpdateTeamAsync(userInput, teamName, cancellationToken);
    //     
    //     return updateRes is null
    //         ? BadRequest("Username is already exists.")
    //         : !updateRes.IsModifiedCountAvailable
    //         ? BadRequest("Update failed. Try again later.")
    //         : Ok("Team has been updated successfully.");
    //
    //     // return updateRes is null || !updateRes.IsModifiedCountAvailable
    //     //     ? BadRequest("Update failed. Try again later.")
    //     //     : Ok(new { message = "Team has been updated successfully." });
    // }
}       