using api.Extensions;
using api.Extensions.Validations;
using api.Repositories.Player;

namespace api.Controllers.Player;

public class UserController(IUserRepository _userRepository, ITokenService _tokenService) : BaseApiController
{
    #region User Management

    [HttpPut]
    public async Task<ActionResult> UpdatePlayer(UserUpdateDto userUpdateDto, CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);
        
        if (userId is null)
            return Unauthorized("You are not logged in. Please login again.");
        
        UpdateResult? updateResult = await _userRepository.UpdatePlayerAsync(userUpdateDto, User.GetHashedUserId(), cancellationToken);

        return updateResult is null || !updateResult.IsModifiedCountAvailable
            ? BadRequest("Update failed. Try again later.")
            : Ok(new { message = "User has been updated successfully." });

        // return testPlayer is null
        //     ? BadRequest("Update failed. Try again later.")
        //     : Ok(testPlayer);
    }

    #endregion

    #region Photo Management

    // only jpeg, png, jpg, Between 250KB(500 * 500) and 4MB(2000 * 2000)
    [HttpPost("add-photo")]
    public async Task<ActionResult<Photo>> AddPhoto(
        [AllowedFileExtensions, FileSize(250_000, 4_000_000)]
        IFormFile file, CancellationToken cancellationToken
    )
    {
        if (file is null) return BadRequest("No file selected with this request");

        Photo? photo = await _userRepository.UploadPhotoAsync(file, User.GetHashedUserId(), cancellationToken);

        return photo is null ? BadRequest("Add photo failed. See logger.") : photo;
    }

    [HttpPut("set-main-photo")]
    public async Task<ActionResult> SetMainPhoto(string photoUrlIn, CancellationToken cancellationToken)
    {
        string? hashedUserId = User.GetHashedUserId();

        if (string.IsNullOrEmpty(hashedUserId))
            return Unauthorized("You are not logged in. Please login again.");

        UpdateResult? updateResult =
            await _userRepository.SetMainPhotoAsync(hashedUserId, photoUrlIn, cancellationToken);

        return updateResult is null || !updateResult.IsModifiedCountAvailable
            ? BadRequest("Update failed. Try again later.")
            : Ok(new { message = "Player has been updated successfully." });
    }

    [HttpPut("delete-photo")]
    public async Task<ActionResult> DeletePhoto(string photoUrlIn, CancellationToken cancellationToken)
    {
        string? hashedUserId = User.GetHashedUserId();

        if (string.IsNullOrEmpty(hashedUserId))
            return Unauthorized("The player is not logged in. Please login again.");

        UpdateResult? updateResult =
            await _userRepository.DeletePhotoAsync(hashedUserId, photoUrlIn, cancellationToken);

        return updateResult is null || !updateResult.IsModifiedCountAvailable
            ? BadRequest("Photo deletion failed. Try again later. if the issue persists, please contact the administrator.")
            : Ok(new { message = "Photo deleted successfully." });
    }
    #endregion
}