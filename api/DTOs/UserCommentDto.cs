namespace api.DTOs;

public record UserCommentDto(
    string CommenterName,
    string CommentedMemberName,
    string Content,
    DateTime CreatedAt
    );