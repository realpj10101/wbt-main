namespace api.Models;

public record ChatMessage(
    [property: BsonId, BsonRepresentation(BsonType.ObjectId)]
    ObjectId? Id, // hamishe sabet
    ObjectId? TeamId,
    string SenderUserName,
    string Message,
    DateTime TimeStamp
);