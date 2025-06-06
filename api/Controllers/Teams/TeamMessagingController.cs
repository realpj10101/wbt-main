using api.Extensions;
using api.Interfaces.Teams;

namespace api.Controllers.Teams;

public class TeamMessagingController(ITeamMessagingRepository _teamMessagingRepository, ITokenService _tokenService) : BaseApiController
{
    public async Task<IActionResult> SendMessage(MessageSenderDto sender, string teamName, CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (userId is null)
            return Unauthorized("You are not logged in. Please login again.");
        
        MessageSenderDto? result = await _teamMessagingRepository.SavedMessageAsync(sender, teamName);
        
        if (result is null)
            return NotFound("Target team was not found.");

        return Ok(result);
    }
}