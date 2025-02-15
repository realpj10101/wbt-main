using api.Extensions;
using api.Helpers;
using api.Interfaces.Player;
using api.Models.Helpers;
using api.Repositories.Player;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers.Player;

[Authorize]
public class CommentController(
    ICommentRepository _commentRepository,
    ITokenService _tokenService, IFollowRepository _followRepository) : BaseApiController
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll([FromQuery] CommentParams commentParams,
        CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (userId is null)
            return Unauthorized("You are not logged in. Please login again.");
        
        commentParams.UserId = userId;
        
        PagedList<AppUser> pagedAppUser = await _commentRepository.GetAllAsync(commentParams, cancellationToken);

        if (pagedAppUser.Count == 0) return NoContent();
        
        Response.AddPaginationHeader(new (
            pagedAppUser.CurrentPage,
            pagedAppUser.PageSize,
            pagedAppUser.TotalItems,
            pagedAppUser.TotalPages
            ));

        List<PlayerDto> playerDtos = [];

        bool isFollowing;
        foreach (AppUser appUser in pagedAppUser)
        {
            isFollowing = await _followRepository.CheckIsFollowingAsync(userId.Value, appUser, cancellationToken);
            
            playerDtos.Add(Mappers.ConvertAppUserToPlayerDto(appUser, isFollowing));
        }

        return playerDtos;
    }

    [HttpGet("get-user-comments/{targetMemberUserName}")]
    public async Task<ActionResult<IEnumerable<UserCommentDto>>> GetUserCommets(string targetMemberUserName,
        CancellationToken cancellationToken)
    {
        List<Comment>? comments = await _commentRepository.GetCommentsByUserNameAsync(targetMemberUserName, cancellationToken);

        if (comments.Count == 0) return NoContent();
        
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);
        
        if (userId is null) return Unauthorized("You are not logged in. Please login again.");

        List<UserCommentDto> userCommentDtos = [];

        foreach (var comment in comments)
        {   
            userCommentDtos.Add(Mappers.ConvertCommentToUserCommentDto(comment));
        }

        return userCommentDtos;
    }
}