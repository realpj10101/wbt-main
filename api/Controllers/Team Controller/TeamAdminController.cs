using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize(Policy = "RequiredAdminRole")]
public class TeamAdminController(ITeamAdminRepository _teamAdminRepository) : BaseApiController
{
    [HttpGet("players-with-roles")]
    public async Task<ActionResult<IEnumerable<PlayerWithRoleDto>>> PlayerWithRole()
    {
        IEnumerable<PlayerWithRoleDto> players = await _teamAdminRepository.GetPlayersWithRolesAsync();

        return !players.Any() ? NoContent() : Ok(players);
    }
}