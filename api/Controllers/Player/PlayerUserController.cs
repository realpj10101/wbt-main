using api.Extensions;
using api.Extensions.Validations;
using api.Repositories.Player;

namespace api.Controllers.Player;

public class PlayerUserController(IPlayerUserRepository _playerUserRepository) : BaseApiController
{
    #region User Management

    [HttpPut]
    public async Task<ActionResult> UpdatePlayer(PlayerUpdateDto playerUpdateDto, CancellationToken cancellationToken)
    {
        UpdateResult? updateResult = await _playerUserRepository.UpdatePlayerAsync(playerUpdateDto, User.GetHashedUserId(), cancellationToken);

        return updateResult is null || !updateResult.IsModifiedCountAvailable
            ? BadRequest("Update failed. Try again later.")
            : Ok(new { message = "User has been updated successfully." });
    }
    
    #endregion

    #region Photo Management

    [HttpPost("add-photo")]
    public async Task<ActionResult<Photo>> AddPhoto(
        [AllowedFileExtensions, FileSize(250_000, 4_000_000)]
        IFormFile file, CancellationToken cancellationToken
    )
    {
        if (file is null) return BadRequest("No file selected with this request");

        Photo? photo = await _playerUserRepository.UploadPhotoAsync(file, User.GetHashedUserId(), cancellationToken);

        return photo is null ? BadRequest("Add photo failed. See logger.") : photo;
    }
    #endregion
}