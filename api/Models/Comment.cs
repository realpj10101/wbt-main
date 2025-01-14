using System.Runtime.InteropServices;

namespace api.Models;

public record Comment(
    [Optional][property: BsonId, BsonRepresentation(BsonType.ObjectId)] ObjectId Id, // optional
    ObjectId CommenterId,
    ObjectId CommentedMemberId,
    string Content,
    DateTime CreatedAt
    );