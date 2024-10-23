using api.DTOs.Team;
using api.Extensions;
using api.Extensions.Validations;
using api.Interfaces.Team;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class TeamUserController(ITeamUserRepository _teamUserRepository) : BaseApiController
{
    [HttpPut]
    public async Task<ActionResult> UpdateTeam(TeamUpdateDto teamUpdateDto, CancellationToken cancellationToken)
    {
        UpdateResult? updateResult = await _teamUserRepository.UpdateTeamAsync(teamUpdateDto, User.GetHashedUserId(), cancellationToken);

        return updateResult is null || !updateResult.IsModifiedCountAvailable
        ? BadRequest("Update failed. Try again later")
        : Ok(new { message = "Team has been updated successfully." });
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<Photo>> AddPhoto(
        [AllowedFileExtensions, FileSize(500 * 500, 2000 * 2000)]
        IFormFile file, CancellationToken cancellationToken
        )
    {
        string? hashedUserId = User.GetHashedUserId();

        if (string.IsNullOrEmpty(hashedUserId))
            return Unauthorized("The team is not logged in");

        if (file is null) return BadRequest("No file is selected with this request");

        Photo? photo = await _teamUserRepository.UploadTeamPhotoAsync(file, User.GetHashedUserId(), cancellationToken);

        return photo is null ? BadRequest("Add photo failed, See logger") : photo;
    }

    [HttpPut("set-main-photo")]
    public async Task<ActionResult<Photo>> SetTeamMainPhoto(string photoUrlIn, CancellationToken cancellationToken)
    {
        string? hashedUserId = User.GetHashedUserId();

        if (string.IsNullOrEmpty(hashedUserId))
            return Unauthorized("The team is not logged in");

        UpdateResult? updateResult = await _teamUserRepository.SetTeamMainPhotoAsync(photoUrlIn, hashedUserId, cancellationToken);

        return updateResult is null || updateResult.ModifiedCount == 0
        ? BadRequest("Set as main photo failed. Try again in a few moments. If the issue presists contant the admin.")
        : Ok(new { message = "set photo as main suceded." });
    }

    [HttpPut("delete-photo")]
    public async Task<ActionResult> deleteTeamPhoto(string photoUrlIn, CancellationToken cancellationToken)
    {
        string? hashedUserId = User.GetHashedUserId();

        if (string.IsNullOrEmpty(hashedUserId))
            return Unauthorized("The team is not logged in");

        UpdateResult? updateResult = await _teamUserRepository.DeleteTeamPhotoAsync(hashedUserId, photoUrlIn, cancellationToken);

        return updateResult is null || updateResult.ModifiedCount == 0
            ? BadRequest("Photo deletion failed. Try again in a few moments. If the issue presists contact the admin.")
            : Ok(new { message = "Photo deleted succeded." });
    }
}