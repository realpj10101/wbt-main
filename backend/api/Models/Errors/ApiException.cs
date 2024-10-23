namespace api.Models.Errors;

public class ApiException
{
    public ObjectId Id { get; set; }
    required public int StatusCode { get; set; }
    required public string? Message { get; set; }
    required public string? Details { get; set; }
    required public DateTime Time { get; set; }
};


// [property: BsonId, BsonRepresentation(BsonType.ObjectId)] ObjectId Id,
// int StatusCode,
// string Message,
// string? Details,
// DateTime Time