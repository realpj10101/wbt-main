using api.Extensions;
using api.Repositories.Player;

namespace api.Controllers.Player;

public class AdminController(IAdminRepository _adminRepository, ITokenService _tokenService) : BaseApiController
{
    [HttpGet("users-with-roles")]
    public async Task<ActionResult<IEnumerable<PlayerWithRoleDto>>> PlayerWithRoles()
    {
        IEnumerable<PlayerWithRoleDto> players = await _adminRepository.GetUsersWithRoleAsync();

        return !players.Any() ? NoContent() : Ok(players);
    }

    [HttpPut("delete-user/{targetUserName}")]
    public async Task<ActionResult> DeleteUser(string targetUserName, CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);
        
        if (playerId is null) return Unauthorized("You are not logged in. Please login again.");
        
        DeleteResult? deleteResult = await _adminRepository.DeleteUserAsync(targetUserName, cancellationToken);
        
        return deleteResult is null
            ? BadRequest("Delete user failed try again.")
            : Ok(new { message = "User deleted successfully." });
    }
}