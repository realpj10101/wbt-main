namespace api.Extensions;

public static class ValidationsExtensions
{
    public static ObjectId? ValidateObjectId(ObjectId? id)
    {
        return id is null || !id.HasValue || id.Equals(ObjectId.Empty)
            ? null
            : id;
    }
}