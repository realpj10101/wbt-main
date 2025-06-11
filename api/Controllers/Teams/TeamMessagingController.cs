using api.DTOs.Helpers;
using api.Extensions;
using api.Interfaces.Teams;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers.Teams;

public class TeamMessagingController(ITeamMessagingRepository _teamMessagingRepository, ITokenService _tokenService) : BaseApiController
{
    [HttpPost("message")]
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

    [HttpGet("message")]
    public async Task<IActionResult> GetMessage()
    {
        List<ChatMessage> messages = await _teamMessagingRepository.GetAllMessagesAsync();

        List<ChatMessageResponse> messagesRes = [];

        foreach (ChatMessage message in messages)
        {
            ChatMessageResponse res = new(
                SenderUserName: message.SenderUserName,
                Message: message.Message,
                TimeStamp: message.TimeStamp
            );

            messagesRes.Add(res);
        }

        return Ok(messagesRes);
    }

    [Authorize(Roles = "coach")]
    [HttpDelete("delete-chats/{teamName}")]
    public async Task<ActionResult<Response>> DeleteChats(string teamName, CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (userId is null)
            return Unauthorized("You are not logged in. Please login again.");

        OperationResult? opResult = await _teamMessagingRepository.DeleteAllMessagesAsync(teamName, cancellationToken);

        return opResult.IsSuccess
            ? Ok(new Response(Message: "Chats delted successfully"))
            : opResult.Error.Code switch
            {
                ErrorCode.TeamNotFound => BadRequest(opResult.Error.Message),
                ErrorCode.ChatNotFound => BadRequest(opResult.Error.Message),
                _ => BadRequest("Something went wrong.")
            };
    }
}