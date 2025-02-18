using api.DTOs.Team_DTOs;
using api.Extensions;
using api.Interfaces.Team;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api.Controllers.Team;

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