using api.Extensions;
using api.Interfaces.Player;

namespace api.Controllers.Player;

public class PlayerDetailsController(IPlayerDetailsRepository _playerDetailsRepository) : BaseApiController
{
    [HttpPut]
    public async Task<ActionResult> UpdatePlayerDetails(PlayerDetailsDto playerDetailsDto,
        CancellationToken cancellationToken)
    {
        UpdateResult? updateResult = await _playerDetailsRepository.UpdatePlayerDetailsAsync(playerDetailsDto, User.GetHashedUserId(), cancellationToken);
        
        return updateResult is null || !updateResult.IsModifiedCountAvailable
            ? BadRequest("Operation failed. Try again later.")
            : Ok(new { message = "Player details registered successfully." });
    }
}