using System.Runtime.InteropServices;

namespace api.Models;

public record Like(
    [Optional][property: BsonId, BsonRepresentation(BsonType.ObjectId)] ObjectId Id,
    ObjectId LikerId,
    ObjectId LikedMemberId
);