namespace api.Extensions;

public class AppVariablesExtensions
{
    public const string TokenKey = "TokenKey";

    public const string collectionPlayers = "users";
    public const string collectionCoaches = "coaches";
    public const string collectionTeams = "teams";
    public const string collectionFollows = "follows";
    public const string collectionLikes = "likes";
    public const string collectionJoines = "joins";
    public const string collectionExceptionLogs = "exception-logs";

    public readonly static string[] AppVersion = ["1", "1.0.2"];

    public readonly static AppRole[] roles = [
        new() {Name = Roles.admin.ToString()},
        new() {Name = Roles.moderator.ToString()},
        new() {Name = Roles.member.ToString()}
    ];
}

public enum Roles
{
    admin, 
    moderator,
    member
}

public enum FollowPredicate
{
    Followings,
    Followers
}

public enum FollowAddOrRemove
{
    IsAdded,
    IsRemoved
}