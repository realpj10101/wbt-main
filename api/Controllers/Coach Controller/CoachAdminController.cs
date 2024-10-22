using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize(Policy = "RequiredCoachAdminRole")]
public class CoachAdminController(ICoachAdminRepository _coachAdminRepository) : BaseApiController
{
    [HttpGet("coaches-with-roles")]
    public async Task<ActionResult<IEnumerable<CoachWithRoleDto>>> CoachesWithRoles()
    {
        IEnumerable<CoachWithRoleDto> coaches = await _coachAdminRepository.GetCoachesWithRolesAsync();

        return !coaches.Any() ? NoContent() : Ok(coaches);  
    }
}
