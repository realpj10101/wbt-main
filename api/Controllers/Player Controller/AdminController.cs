    using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize(Policy = "RequiredAdminRole")]
public class AdminController(IAdminRepository _adminRepository) : BaseApiController
{
    [HttpGet("players-with-roles")]
    public async Task<ActionResult<IEnumerable<PlayerWithRoleDto>>> PlayersWithRole()
    {
        IEnumerable<PlayerWithRoleDto> players = await _adminRepository.GetPlayersWithRolesAsync();

        return !players.Any() ? NoContent() : Ok(players);
    }
}
