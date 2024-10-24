using api.Enums;

namespace api.Helpers;

public class FollowParams : PaginationParams
{   
    public ObjectId? UserId { get; set; }
    // public FollowPredicate Predicate { get; set; } = FollowPredicate.Followings;

    public FollowPredicateEnum Predicate { get; set; }
}