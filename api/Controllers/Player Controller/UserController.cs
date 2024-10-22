using api.Extensions;
using api.Extensions.Validations;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class UserController(IUserRepository _userRepository) : BaseApiController
{
    [HttpPut]
    public async Task<ActionResult> UpdatePlayer(PlayerUpdateDto playerUpdateDto, CancellationToken cancellationToken)
    {
        UpdateResult? updateResult = await _userRepository.UpdatePlayerAsync(playerUpdateDto, User.GetHashedUserId(), cancellationToken);

        return updateResult is null || !updateResult.IsModifiedCountAvailable
        ? BadRequest("Update failed. Try again later.")
        : Ok(new { message = "Player has been updated successfully." });
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<Photo>> AddPhoto(
        [AllowedFileExtensions, FileSize(500 * 500, 2000 * 2000)]
        IFormFile file, CancellationToken cancellationToken
        )
    {
        string? hashedUserId = User.GetHashedUserId();

        if (string.IsNullOrEmpty(hashedUserId))
            return Unauthorized("The player is not logged in");

        if (file is null) return BadRequest("No file is selected with this request.");

        Photo? photo = await _userRepository.UploadPhotoAsync(file, User.GetHashedUserId(), cancellationToken);

        return photo is null ? BadRequest("Add photo failed, See logger") : photo;
    }

    [HttpPut("set-main-photo")]
    public async Task<ActionResult> SetMainPhoto(string photoUrlIn, CancellationToken cancellationToken)
    {
        string? hashedUserId = User.GetHashedUserId();

        if (string.IsNullOrEmpty(hashedUserId))
            return Unauthorized("The player is not logged in");

        UpdateResult? updateResult = await _userRepository.SetMainPhotoAsync(hashedUserId, photoUrlIn, cancellationToken);

        return updateResult is null || updateResult.ModifiedCount == 0
        ? BadRequest("Set as main photo failed. Try again in a few moments. If the issue presists contant the admin.")
        : Ok(new { message = "Set this photo as main succeded." });
    }

    [HttpPut("delete-photo")]
    public async Task<ActionResult> DeletePhoto(string photoUrlIn, CancellationToken cancellationToken)
    {
        string? hashedUserId = User.GetHashedUserId();

        if (string.IsNullOrEmpty(hashedUserId))
            return Unauthorized("The player is not logged in");

        UpdateResult? updateResult = await _userRepository.DeletePhotoAsync(hashedUserId, photoUrlIn, cancellationToken);

        return updateResult is null || updateResult.ModifiedCount == 0
            ? BadRequest("Photo deletion failed. Try again in a few moments. If the issue presists contact the admin")
            : Ok(new { message = "Photo deleted successfully." });
    }
}
