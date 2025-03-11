using api.DTOs.Helpers;

namespace api.Extensions;

public static class ValidationsExtensions
{
    public static ObjectId? ValidateObjectId(ObjectId? objectId)
    {
        return objectId is null || !objectId.HasValue || objectId.Equals(ObjectId.Empty)
            ? null
            : objectId;
    }

    public static OperationResult<bool> ValidateExObjectId(ObjectId? objectId) =>
        new(
            objectId.HasValue && !objectId.Equals(ObjectId.Empty)
        );
}