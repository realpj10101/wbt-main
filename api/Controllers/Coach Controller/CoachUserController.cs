using api.Extensions;
using api.Extensions.Validations;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class CoachUserController(ICoachUserRepository _coachUserRepository) : BaseApiController
{

    [HttpPut]
    public async Task<ActionResult> UpdateCoach(CoachUpdateDto coachUpdateDto, CancellationToken cancellationToken)
    {
        UpdateResult? updateResult = await _coachUserRepository.UpdateCoachAsync(coachUpdateDto, User.GetHashedUserId(), cancellationToken);

        return updateResult is null || updateResult.ModifiedCount == 0
            ? BadRequest("Update failed. Try again later")
            : Ok(new { message = "Coach has been updated succesfully"});
    }

    [HttpPost("add-coach-photo")]
    public async Task<ActionResult<Photo>> AddCoachPhoto(
        [AllowedFileExtensions, FileSize(500 * 500, 2000 * 2000)]
        IFormFile file, CancellationToken cancellationToken
    )
    {
        if (file is null) return BadRequest("no file is  selected with this request.");

        Photo? photo = await _coachUserRepository.UploadCoachPhotoAsync(file, User.GetHashedUserId(), cancellationToken);
        
        return photo is null ? BadRequest("Add photo failed. See logger") : photo;  
    }

    [HttpPut("set-coach-main-photo")]
    public async Task<ActionResult> SetCoachMainPhoto(string photoUrlIn, CancellationToken cancellationToken)
    {
        UpdateResult? updateResult = await _coachUserRepository.SetMainCoachPhotoAsync(User.GetHashedUserId(), photoUrlIn, cancellationToken);

        return updateResult is null || updateResult.ModifiedCount == 0
            ? BadRequest("Set as main photo failed. Try again i a few moments.")
            : Ok(new {message = "Set this photo as main succeded"});
    }

    [HttpPut("delete-coach-photo")]
    public async Task<ActionResult> DeleteCoachPhoto(string photoUrlIn, CancellationToken cancellationToken)
    {
        UpdateResult? updateResult = await _coachUserRepository.DeleteCoachPhotoAsync(User.GetHashedUserId(), photoUrlIn, cancellationToken);

        return updateResult is null || updateResult.ModifiedCount == 0
        ? BadRequest("Photo deletion failed. Try again in a few moments")
        : Ok(new { message = "Photo deleted successfully"});
    }
}
