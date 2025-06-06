namespace api.Models;

public record ChatMessage(
    [property: BsonId, BsonRepresentation(BsonType.ObjectId)]
    string? Id, // hamishe sabet
    ObjectId? TeamId,
    string SenderUserName,
    string Message,
    DateTime TimeStamp
);