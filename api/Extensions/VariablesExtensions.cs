namespace api.Extensions;

public class AppVariablesExtensions
{
    public const string TokenKey = "TokenKey";

    public const string collectionUsers = "users";
    public const string collectionFollows = "follows";
    public const string CollectionComments = "comments";
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