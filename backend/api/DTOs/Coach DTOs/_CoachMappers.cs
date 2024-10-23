        using System.Security.Cryptography;
using api.Extensions;

namespace api.DTOs;

public static class CoachMappers
{
    public static RootModel ConvertRegisterCoDtoToRootModel(RegisterCoachDto coachInput)
{
        return new RootModel
        {
            Email = coachInput.Email,
            UserName = coachInput.UserName,
            Name = coachInput.Name.ToLower().Trim(),
            LastName = coachInput.LastName.ToLower().Trim(),
            NationalCode = coachInput.NationalCode.Trim(),
            Age = coachInput.Age,
            KnownAs = coachInput.KnownAs.Trim(),
            LastActive = DateTime.UtcNow,
            Gender = coachInput.Gender.ToLower(),
            City = coachInput.City.Trim(),
            Country = coachInput.Country.Trim(),
            Records = coachInput.Records.Trim().ToLower(),
            Photos = []
        };
    }

    public static LoggedInCoachDto ConvertRootModelToLoggedInCoDto(RootModel rootModel, string tokenValue)
    {
        return new LoggedInCoachDto
        {
            Email = rootModel.Email,
            Token = tokenValue,
            KnownAs = rootModel.KnownAs,
            Gender = rootModel.Gender,
            ProfilePhotoUrl = rootModel.Photos.FirstOrDefault(photo => photo.IsMain)?.Url_165
        };
    }

    public static CoachDto ConvertRootModelToCoachDto(RootModel rootModel, bool isFollowing = false)
    {
        return new CoachDto(
            // Id: rootModel.Id.ToString(),
            Email: rootModel.NormalizedEmail!,
            Name: rootModel.Name,
            LastName: rootModel.LastName,
            NationalCode: rootModel.NationalCode,
            Age: CustomDateTimeExtensions.CalculateAge(rootModel.Age),
            KnownAs: rootModel.KnownAs,
            Created: rootModel.CreatedOn,
            Lastactive: rootModel.LastActive,
            Gender: rootModel.Gender,
            LookingFor: rootModel.LookingFor,
            City: rootModel.City,
            Country: rootModel.Country,
            Photos: rootModel.Photos,
            Records: rootModel.Records,
            IsFollowing: isFollowing,
            IsAdmin: rootModel.IsAdmin
        );
    }

    public static Photo ConvertPhotoUrlsToPhoto(string[] photoUrls, bool isMain)
    {
        return new Photo(
            Url_165: photoUrls[0],
            Url_256: photoUrls[1],
            Url_enlarged: photoUrls[2],
            IsMain: isMain
        );
    }

    public static Follow ConvertFollowIdsToFollow(ObjectId followerId, ObjectId followedId)
    {
        return new Follow(
            FollowerId: followerId,
            FollowedMemberId: followedId
        );
    }

    public static Like ConvertLikeIdsToLike(ObjectId likerId, ObjectId likedId)
    {
        return new Like(
            LikerId: likerId,
            LikedMemberId: likedId
        );
    }
}
