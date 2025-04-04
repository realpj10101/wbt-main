using System.Security.Cryptography;
using api.DTOs.Team_DTOs;
using api.Extensions;
using SharpCompress.Common;

namespace api.DTOs;

public static class Mappers
{
    public static AppUser ConvertRegisterDtoToAppUser(RegisterDto playerInput)
    {
        return new AppUser
        {
            Email = playerInput.Email,
            UserName = playerInput.UserName,
            DateOfBirth = playerInput.DateOfBirth,
            Gender = playerInput.Gender,
            LastActive = DateTime.UtcNow,
            Photos = []
        };
    }

    public static LoggedInDto ConvertAppUserToLoggedInDto(AppUser appUser, string tokenValue)
    {
        return new LoggedInDto
        {
            Token = tokenValue,
            UserName = appUser.NormalizedUserName,
            Gender = appUser.Gender,
            ProfilePhotoUrl = appUser.Photos.FirstOrDefault(photo => photo.IsMain)?.Url_165
        };
    }

    public static PlayerDto ConvertAppUserToPlayerDto(AppUser appUser, bool isFollowing = false, bool isLiking = false, bool isCaptian = false)
    {
        return new PlayerDto(
            // Id: rootModel.Id!.ToString(),    
            UserName: appUser.NormalizedUserName!,
            Name: appUser.Name,
            LastName: appUser.LastName,
            Age: CustomDateTimeExtensions.CalculateAge(appUser.DateOfBirth),
            Height: appUser.Height,
            Weight: appUser.Weight,
            Position: appUser.Position,
            ExperienceLevel: appUser.ExperienceLevel,
            Skills: appUser.Skills,
            GamesPlayed: appUser.GamesPlayed,
            PointsPerGame: appUser.PointsPerGame,
            ReboundsPerGame: appUser.ReboundsPerGame,
            AssistsPerGame: appUser.AssistsPerGame,
            Bio: appUser.Bio,
            Achievements: appUser.Achievements,
            Created: appUser.CreatedOn,
            LastActive: appUser.LastActive,
            Gender: appUser.Gender,
            City: appUser.City,
            Region: appUser.Region,
            Country: appUser.Country,
            Photos: appUser.Photos,
            IsFollowing: isFollowing,
            IsCaptain: isCaptian,
            IsLiking: isLiking
        );
    }

    public static TestPlayer ConvertPlayerUpdateDtoToTestPlayer(PlayerUpdateDto playerUpdateDto)
    {
        return new TestPlayer(
            Name: playerUpdateDto.Name,
            LastName: playerUpdateDto.LastName
        );
    }

    public static Photo ConvertPhotoUrlsToPhoto(string[] photoUrls, bool isMain)
    {
        return new Photo(
            Url_165: photoUrls[0],
            Url_256: photoUrls[1],
            Url_enlarged: photoUrls[2],
            IsMain: isMain
        );
    }

    //Follow
    public static Follow ConvertFollowIdsToFollow(ObjectId followerId, ObjectId followedId)
    {
        return new Follow(
            FollowerId: followerId,
            FollowedMemberId: followedId
        );
    }

    public static Like ConvertLikeIdsToLike(ObjectId likerId, ObjectId likedId)
    {
        return new Like(
            LikerId: likerId,
            LikedMemberId: likedId
        );
    }

    public static Comment ConvertCommentIdsToComment(ObjectId commenterId, ObjectId commentedId, string commenterName, string commentedMemberName, string content)
    {
        return new Comment(
            CommenterId: commenterId,
            CommentedMemberId: commentedId,
            CommenterName: commenterName,
            CommentedMemberName: commentedMemberName,
            Content: content,
            CreatedAt: DateTime.Now
            );
    }

    public static Team ConvertCreateTeamDtoToTeam(ObjectId userId, CreateTeamDto userInput)
    {
        return new Team(
            CreatorId: userId,
            MembersIds: [],
            MembersUserNames: [],
            TeamName: userInput.TeamName.ToLower(),
            TeamLevel: userInput.TeamLevel.ToLower(),
            Achievements: userInput.Achievements.ToLower(),
            GamesPlayed: userInput.GamesPlayed,
            GamesWon: userInput.GamesWon,
            GamesLost: userInput.GamesLost,
            CreatedAt: DateTime.UtcNow
        );
    }

    public static ShowTeamDto ConvertTeamToShowTeamDto(Team team)
    {
        return new ShowTeamDto
        {
            TeamName = team.TeamName,
            MembersUserNames = team.MembersUserNames,
            TeamLevel = team.TeamLevel,
            Achievements = team.Achievements,
            GamesPlayed = team.GamesPlayed,
            GamesWon = team.GamesWon,
            GamesLost = team.GamesLost,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Team ConvertUpdateTeamDtoToTeam(ObjectId userId, UpdateTeamDto userInput)
    {
        return new Team(
            CreatorId: userId,
            MembersIds: [],
            MembersUserNames: [],
            TeamName: userInput.TeamName.ToLower(),
            TeamLevel: userInput.TeamLevel.ToLower(),
            Achievements: userInput.Achievements.ToLower(),
            GamesPlayed: userInput.GamesPlayed,
            GamesWon: userInput.GamesWon,
            GamesLost: userInput.GamesLost,
            CreatedAt: DateTime.UtcNow
        );
    }

    public static EnrolledTeam ConvertTeamToEnrolledTeamDto(Team? team)
    {
        if (team is null)
        {
            throw new ArgumentNullException(nameof(team), "Team cannot be null");
        }
        
        return new EnrolledTeam(
            TeamId: team.Id,
            TeamName: team.TeamName
            );
    }

    public static UserCommentDto ConvertCommentToUserCommentDto(Comment comment)
    {
        return new UserCommentDto(
            CommenterName: comment.CommenterName,
            CommentedMemberName: comment.CommentedMemberName,
            Content: comment.Content,
            CreatedAt: comment.CreatedAt
        );
    }
}
