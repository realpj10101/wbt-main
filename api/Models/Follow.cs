using System.Runtime.InteropServices;

namespace api.Models;

public record Follow(
   [Optional][property: BsonId, BsonRepresentation(BsonType.ObjectId)] ObjectId Id,
   ObjectId FollowerId,
   ObjectId FollowedMemberId
);