using api.DTOs.Helpers;
using api.DTOs.Team_DTOs;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Player;
using api.Interfaces.Teams;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NuGet.Protocol.Plugins;

namespace api.Controllers.Teams;

[Authorize]
public class TeamController(
    ITeamRepository _teamRepository,
    IFollowRepository _followRepository,
    ILikeRepository _likeRepository,
    ITokenService _tokenService) : BaseApiController
{
    [HttpPost("create")]
    public async Task<ActionResult<ShowTeamDto>> Create(CreateTeamDto userInput, CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (userId is null) return Unauthorized("You are not logged in. Please login again.");

        if ((userInput.GamesWon + userInput.GamesLost) > userInput.GamesPlayed)
            return BadRequest("Games won and games lost are more than games played.");

        ShowTeamDto? showTeamDto = await _teamRepository.CreateAsync(userId.Value, userInput, cancellationToken);

        return showTeamDto is not null
            ? Ok(showTeamDto)
            : showTeamDto is null
                ? BadRequest("Team is already exists")
                : BadRequest("Create team failed. try again or contact administrator.");
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShowTeamDto>>> GetAll([FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        PagedList<Team>? pagedTeams = await _teamRepository.GetAllAsync(paginationParams, cancellationToken);

        if (pagedTeams.Count == 0)
            return NoContent();

        PaginationHeader paginationHeader = new(
            CurrentPage: pagedTeams.CurrentPage,
            ItemsPerPage: pagedTeams.PageSize,
            TotalItems: pagedTeams.TotalItems,
            TotalPages: pagedTeams.TotalPages
        );

        Response.AddPaginationHeader(paginationHeader);

        string? userIdHashed = User.GetHashedUserId();

        ObjectId? userId = await _tokenService.GetActualUserIdAsync(userIdHashed, cancellationToken);

        if (userId is null) return Unauthorized("You are not logged in. Please login again.");

        List<ShowTeamDto> playerDtos = [];

        foreach (Team team in pagedTeams)
        {
            playerDtos.Add(Mappers.ConvertTeamToShowTeamDto(team));
        }

        return playerDtos;
    }

    // [Authorize(Roles = "coach")]
    // [HttpPut("update-team/{teamName}")]
    // public async Task<ActionResult<Response>> Update(UpdateTeamDto userInput, string teamName,
    //     CancellationToken cancellationToken)
    // {
    //     ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);
    //
    //     if (userId is null)
    //         return Unauthorized("You are not logged in. Please login again.");
    //
    //     TeamStatus? tS = await _teamRepository.UpdateTeamAsync(userId.Value, userInput, teamName, cancellationToken);
    //
    //     return tS.IsSuccess
    //         ? Ok(new Response(Message: $"You updated the team {teamName} successfully."))
    //         : tS.IsTargetMemberNotFound
    //             ? NotFound($"{userInput.UserName} is not found.")
    //             : tS.IsTargetTeamNotFound
    //                 ? NotFound($"Team {teamName} is not found.")
    //                 : tS.IsAlreadyJoined
    //                     ? BadRequest($"{userInput.UserName} is already joined.")
    //                     : tS.IsJoiningThemself
    //                         ? BadRequest("You are the owner of this team.")
    //                         : BadRequest("Updated team failed. Please contact administrator.");
    // }

    [HttpGet("get-by-name/{teamName}")]
    public async Task<ActionResult<ShowTeamDto>> GetByName(string teamName, CancellationToken cancellationToken)
    {
        ShowTeamDto? teamDto = await _teamRepository.GetByTeamNameAsync(teamName, cancellationToken);

        if (teamDto is null) return NotFound("No team with this Name was found.");

        return teamDto;
    }

    [HttpGet("get-members/{teamName}")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll(string teamName,
        CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (userId is null)
            return Unauthorized("You are not logged in. Please login again.");

        List<AppUser>? appUsers = await _teamRepository.GetTeamMembersAsync(teamName, cancellationToken);

        if (appUsers is null)
            return BadRequest("Target team name was not found.");

        if (appUsers.Count == 0) return NoContent();

        List<PlayerDto> playerDtos = [];

        bool isFollowing;
        bool isLiking;
        foreach (AppUser appUser in appUsers)
        {
            isFollowing = await _followRepository.CheckIsFollowingAsync(userId.Value, appUser, cancellationToken);

            isLiking = await _likeRepository.CheckIsLikingAsync(userId.Value, appUser, cancellationToken);

            playerDtos.Add(Mappers.ConvertAppUserToPlayerDto(appUser, isFollowing, isLiking));
        }

        return playerDtos;
    }

    [Authorize(Roles = "coach")]
    [HttpPut("add-member/{teamName}/{targetMemberUserName}")]
    public async Task<ActionResult<Response>> AddMember(string targetMemberUserName, string teamName,
        CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (userId is null)
            return Unauthorized("You are not logged in. Please login again.");

        TeamStatus? tS =
            await _teamRepository.AddMemberAsync(userId.Value, targetMemberUserName, teamName, cancellationToken);

        return tS.IsSuccess
            ? Ok(new Response(Message: $"You add {targetMemberUserName} to team {teamName} successfully."))
            : tS.IsNotTheCreator
                ? BadRequest("You are not the owner of the team.")
                : tS.IsTargetMemberNotFound
                    ? NotFound($"{targetMemberUserName} is not found.")
                    : tS.IsTargetTeamNotFound
                        ? NotFound($"Team {teamName} is not found.")
                        : tS.IsAlreadyJoined
                            ? BadRequest($"{targetMemberUserName} is already joined.")
                            : tS.IsJoiningThemself
                                ? BadRequest("You are the owner of this team.")
                                : BadRequest("Updated team failed. Please contact administrator.");
    }

    [Authorize(Roles = "coach")]
    [HttpGet("get-team-name")]
    public async Task<ActionResult<Response>> GetTeamName(CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (userId is null)
            return Unauthorized("You are not logged in. Please login again.");

        string? teamName = await _teamRepository.GetTeamNameByIdAsync(userId.Value, cancellationToken);

        return teamName is not null
            ? Ok(new Response(teamName))
            : NoContent();
    }

    [Authorize(Roles = "coach")]
    [HttpPost("assign-captain/{targetUserName}")]
    public async Task<ActionResult<Response>> AssignCaptain(string targetUserName,
        CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (userId is null)
            return Unauthorized("You are not logged in. Please login again.");

        OperationResult opResult = await _teamRepository.AssignCaptainAsync(userId.Value, targetUserName, cancellationToken);

        return opResult.IsSuccess
            ? Ok(new Response( Message: opResult.Message))
            : opResult.Error.Code switch
            {
                ErrorCode.CoachNotFound => BadRequest("Coach not found."),
                ErrorCode.CoachHasNoTeam => BadRequest("Coach has no team."),
                ErrorCode.OnlyOneCaptain => BadRequest("Only one captain is allowed."),
                ErrorCode.UserNotFound => BadRequest($"{targetUserName} not found."),
                ErrorCode.NotInTeam => BadRequest($"{targetUserName} is not in any team."),
                ErrorCode.NotTeamMember => BadRequest($"{targetUserName} is not a team member."),
                ErrorCode.AlreadyCaptain => BadRequest("This user is already a captain."),
                _ => BadRequest("An error occured. Try again or contact administrator")
            };
    }

    [Authorize(Roles = "coach")]
    [HttpDelete("remove-captain/{targetUserName}")]
    public async Task<ActionResult<Response>> RemoveCaptain(string targetUserName, CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(User.GetHashedUserId(), cancellationToken);

        if (userId is null)
            return Unauthorized("You are not logged in. Please login again.");

        OperationResult opResult = await _teamRepository.RemoveCaptainAsync(userId.Value, targetUserName, cancellationToken);

        return opResult.IsSuccess
            ? Ok(new Response(Message: opResult.Message))
            : opResult.Error.Code switch
            {
                ErrorCode.CoachNotFound => BadRequest("Coach has not found"),
                ErrorCode.CoachHasNoTeam => BadRequest("Coach has no team"),
                ErrorCode.UserNotFound => BadRequest($"{targetUserName} not found"),
                ErrorCode.NotInTeam => BadRequest($"{targetUserName} is not in any team."),
                ErrorCode.NotTeamMember => BadRequest($"{targetUserName} is not a team member."),
                ErrorCode.IsNotCaptain => BadRequest("This user is not a captain."),
                _ => BadRequest("An error occured. Try again or contact administrator")
            };
    }
}

// old code
// [HttpPut("update-team/{teamName}")]
// public async Task<ActionResult> Update(UpdateTeamDto userInput, string teamName, CancellationToken cancellationToken)
// {
//     UpdateResult? updateRes = await _teamRepository.UpdateTeamAsync(userInput, teamName, cancellationToken);
//     
//     return updateRes is null
//         ? BadRequest("Username is already existsAc.")
//         : !updateRes.IsModifiedCountAvailable
//         ? BadRequest("Update failed. Try again later.")
//         : Ok("Team has been updated successfully.");
//
//     // return updateRes is null || !updateRes.IsModifiedCountAvailable
//     //     ? BadRequest("Update failed. Try again later.")
//     //     : Ok(new { message = "Team has been updated successfully." });
// }