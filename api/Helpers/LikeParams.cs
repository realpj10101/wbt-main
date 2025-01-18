using api.Enums;

namespace api.Helpers;

public class LikeParams : PaginationParams
{
    public ObjectId? UserId { get; set; }
    
    public LikePredicateEnum Predicate { get; set; }
}