using api.DTOs.Helpers;
using api.DTOs.Team_DTOs;
using api.Extensions;
using api.Repositories.Player;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers.Player;

// [Authorize(Roles = "admin")]
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

    [HttpPut("update-verified-status/{targetTeamName}")]
    public async Task<ActionResult<ShowTeamDto>> UpdateVerifiedStatus(string targetTeamName,
        CancellationToken cancellationToken)
    {
        // ObjectId? adminId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);
        //
        // if (adminId is null)
        //     return Unauthorized("You are not logged in. Please login again.");

        OperationResult<ShowTeamDto> teamResult =
            await _adminRepository.UpdateVerifiedStatus(targetTeamName, cancellationToken);

        return teamResult.IsSuccess
            ? Ok(teamResult.Result)
            : teamResult.Error.Code switch
            {
                _ => BadRequest("Team not found")
            };
    }

    [HttpPut("update-reject-status/{targetTeamName}")]
    public async Task<ActionResult<ShowTeamDto>> UpdateRejectStatus(string targetTeamName, UpdateRejectStatus reason,
        CancellationToken cancellationToken)
    {
        OperationResult<ShowTeamDto> teamResult = 
            await _adminRepository.UpdateRejectStatus(targetTeamName, reason, cancellationToken);
        
        return teamResult.IsSuccess
            ? Ok(teamResult.Result)
            : teamResult.Error.Code switch
            {
                _ => BadRequest("Team not found")
            };
    }
}