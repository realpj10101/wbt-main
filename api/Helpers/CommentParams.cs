using api.Enums;

namespace api.Helpers;

public class CommentParams : PaginationParams
{
    public ObjectId? UserId { get; set; }
    public CommentPredicateEnum Predicate { get; set; }
}