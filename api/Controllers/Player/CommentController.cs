using api.Extensions;
using api.Models.Helpers;
using api.Repositories.Player;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers.Player;

[Authorize]
public class CommentController(ICommentRepository _commentRepository, ITokenService _tokenService) : BaseApiController
{
    // Add Comment
    [HttpPost("add/{targetMemberUserName}")]
    public async Task<ActionResult<Response>> Create(
        string targetMemberUserName,
        [FromBody] CreateCommentDto content,
        CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);
    
        if (userId is null)
            return Unauthorized("You are not logged in. Please login again.");
    
        CommentStatus cS =
            await _commentRepository.CreateAsync(userId.Value, targetMemberUserName, content.Content,
                cancellationToken);
        
        return cS.IsSuccess
            ? Ok(new Response(Message: $"You Commented for {targetMemberUserName} successfully."))
            : cS.IsTargetMemberNotFound
            ? NotFound($"{targetMemberUserName} was not found.")
            : BadRequest("An error occured. Try again or contact the administrator.");
    }

    [HttpDelete("remove/{targetMemberUserName}")]
    public async Task<ActionResult<Response>> Delete(string targetMemberUserName, CancellationToken cancellationToken)
    {
        // Get logged in use ObjectId from token
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (userId is null)
            return Unauthorized("You are not logged in. Please login again.");
        
        CommentStatus cS = await _commentRepository.DeleteAsync(userId.Value, targetMemberUserName, cancellationToken);
        
        return cS.IsSuccess
            ? Ok(new Response(Message: $"You Deleted your comment for {targetMemberUserName} successfully."))
            : cS.IsTargetMemberNotFound
            ? NotFound($"{targetMemberUserName} was not found.")
            : BadRequest($"An error occured. Try again or contact the administrator.");
        
    }
}