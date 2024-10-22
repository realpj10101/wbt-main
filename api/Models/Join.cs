using System.Runtime.InteropServices;

namespace api.Models;

public record Join(

   [Optional][property: BsonId, BsonRepresentation(BsonType.ObjectId)] ObjectId Id,
    ObjectId JoinerId,
    ObjectId JoinedTeamId
);