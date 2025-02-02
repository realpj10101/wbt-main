using api.DTOs.Team_DTOs;
using api.Extensions;
using api.Interfaces.Team;

namespace api.Controllers.Team;

public class TeamController(ITeamRepository _teamRepository, ITokenService _tokenService) : BaseApiController
{
    [HttpPost("create")]
    public async Task<ActionResult<CreateTeamDto>> Create(CreateTeamDto userInput, CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);
        
        if (userId is null) return Unauthorized("You are not logged in. Please login again.");

        CreateTeamDto? createTeamDto = await _teamRepository.CreateAsync(userId.Value, userInput, cancellationToken);
        
        return createTeamDto is not null
            ? Ok(createTeamDto)
            : BadRequest("Create team failed. try again or contact administrator.");
    }
}   